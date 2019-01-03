using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class Beatmap

    ////Loaded data
{
    //Metadata
    public string SongName;
    public string RomanizedSongName;
    public string DifficultyName;
    public string Author;

    //Data
    public Note[] Notes;
    [XmlIgnore]
    public Texture2D BackgroundImage = null;
    public string BackgroundPath;
    [XmlIgnore]
    public AudioClip Song;
    public string SongPath;

    public string version;

    //Modifiers
    public uint SliceCount;
    public float AccMod;
    public float SpeedMod;
    public int Bpm;

    //Accessors
    //FUTURE ME, DO NOT CHANGE THIS, ITS CORRECT, 3 <= 3 is true, meaning there are notes left when there arent
    public bool AnyNotesLeft => PlayedNotes.Count() < Notes.Length;
    public string SanitizedName => Helper.SanitizeString(RomanizedSongName);

    //Methods
    public Note GetLatestForSlice(int slice)
    {
        for (int i = 0; i < CurrentlyLoaded.Notes.Length; i++)
        {
            Note note = CurrentlyLoaded.Notes[i];
            if (note.slice == slice)
            {
                return note;
            }
        }
        throw new Exception();
    }
    public int GetIndexForNote(Note note)
    {
        for (int i = 0; i < CurrentlyLoaded.Notes.Length; i++)
        {
            Note tempNote = CurrentlyLoaded.Notes[i];
            if (tempNote == note)
            {
                return i;
            }
        }
        throw new Exception("Couldnt find note " + note);
    }
    /// <summary>
    /// IEnumerable which contains the notes that have been clicked/played already.
    /// Useful for getting amount of notes clicked using Linq's Count method
    /// </summary>
    public IEnumerable<Note> PlayedNotes => Notes.Where(note => note.clicked);

    public static Beatmap CurrentlyLoaded { get; private set; }

    private static IEnumerable<Note> RemoveJumpsAndResetNotes(IEnumerable<Note> notes)
    {
        List<Note> toExclude = new List<Note>();
        foreach (Note note in notes)
        {
            //Clean up jumps
            var jumpPairs = notes.Where(other => other.slice != note.slice && other.time == note.time);
            if (jumpPairs.Count() > 0)
            {
                toExclude.AddRange(jumpPairs);
            }

            if (!toExclude.Contains(note))
                yield return new Note(note.time, note.slice);
        }
    }
    private void ResetNotes()
    {
        Note[] notes = new Note[Notes.Length];
        for(int i = 0; i < notes.Length; i++)
        {
            Note note = Notes[i];
            notes[i] = new Note(new Time(ms: note.time.Ms), note.slice);
        }
        Notes = notes;
    }

    /// <param name="loadedBackground">If you already have the background texture loaded, pass it here</param>
    /// <param name="loadedSong">If you already have the song loaded, pass it here</param>
    public static void Load(Beatmap map, Texture2D loadedBackground = null, AudioClip loadedSong = null)
    {
        if (!map.hasBeenFixed)
            map.Fix();
        else
            map.ResetNotes();

        CurrentlyLoaded = map;

        map.Song = loadedSong ?? map.Song ?? GetAudio(map.SongPath);
        map.BackgroundImage = loadedBackground ?? map.BackgroundImage ?? Helper.LoadPNG(map.BackgroundPath);

        Debug.Log($"Loaded song {map.SongName}({map.GetUUID()})");
    }
    bool hasBeenFixed = false;
    public void Fix()
    {
        Notes = RemoveJumpsAndResetNotes(Notes).ToArray();

        hasBeenFixed = true;
    }

    public void Reload()
    {
        Load(this);
    }

    /* From http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html */
    /* From https://gamedev.stackexchange.com/a/114886 */
    public class WAV
    {

        // convert two bytes to one float in the range -1 to 1
        static float BytesToFloat(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);
            // convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }

        static int BytesToInt(byte[] bytes, int offset = 0)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value |= ((int)bytes[offset + i]) << (i * 8);
            }
            return value;
        }
        // properties
        public float[] LeftChannel { get; internal set; }
        public float[] RightChannel { get; internal set; }
        public int ChannelCount { get; internal set; }
        public int SampleCount { get; internal set; }
        public int Frequency { get; internal set; }

        public WAV(byte[] wav)
        {

            // Determine if mono or stereo
            ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

            // Get the frequency
            Frequency = BytesToInt(wav, 24);

            // Get past all the other sub chunks to get to the data subchunk:
            int pos = 12;   // First Subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.
            SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
            if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

            // Allocate memory (right will be null if only mono sound)
            LeftChannel = new float[SampleCount];
            if (ChannelCount == 2) RightChannel = new float[SampleCount];
            else RightChannel = null;

            // Write to double array/s:
            int i = 0;
            while (pos < wav.Length)
            {
                LeftChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                if (ChannelCount == 2)
                {
                    RightChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }
        }

        public override string ToString()
        {
            return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
        }
    }
    class Mp3
    {
        private static MemoryStream AudioMemStream(WaveStream waveStream)
        {
            MemoryStream outputStream = new MemoryStream();
            using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat))
            {
                byte[] bytes = new byte[waveStream.Length];
                waveStream.Position = 0;
                waveStream.Read(bytes, 0, (int)waveStream.Length);
                waveFileWriter.Write(bytes, 0, bytes.Length);
                waveFileWriter.Flush();
            }
            return outputStream;
        }
        public static AudioClip GetMp3Audio(string name, byte[] data)
        {
            // Load the data into a stream
            MemoryStream mp3stream = new MemoryStream(data);
            // Convert the data in the stream to WAV format
            Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);
            // Convert to WAV data
            WAV wav = new WAV(AudioMemStream(mp3audio).ToArray());
            Debug.Log(wav);
            AudioClip audioClip = AudioClip.Create(name, wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            // Return the clip
            return audioClip;
        }
    }
    public static AudioClip GetAudio(string path)
    {
        string extension = Path.GetExtension(path);
        byte[] data = File.ReadAllBytes(path);
        switch (extension)
        {
            case ".mp3":
            {
                return Mp3.GetMp3Audio(Path.GetFileName(path), data);
            }
            case ".wav":
                return WavUtility.ToAudioClip(data);
            default:
            {
                Debug.Log(extension + " file type is not yet supported");
                return null;
            }
        }
    }

    public string GetUUID()
    {
        StringBuilder builder = new StringBuilder((Notes.Length * 20) + SongName.Length);
        foreach(Note note in Notes)
        {
            builder.Append(note.GetUUID());
        }
        builder.Append(SongName);
        return HashSha256(builder.ToString());
    }

    static string HashSha256(string input)
    {
        byte[] crypto = new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
        var hash = new StringBuilder(crypto.Length);
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }

    class Mania
    {
        private static IEnumerable<Note> ToPolymaniaNotes(int keyCount, List<OsuParsers.Beatmaps.Objects.HitObject> hitObjects)
        {
            int sliceWidth = 512 / keyCount;
            foreach (var hitObject in hitObjects)
            {
                //TODO For the 7K + 1 mode, the column index is 1 + x / (512 / 7), leaving the column 0 for the specials.
                int slice = hitObject.Position.X / sliceWidth;
                int time = hitObject.StartTime;

                yield return new Note(new Time(ms: time), (uint)slice);
            }
        }
        public static Beatmap FromFilePath(string osuFilePath)
        {
            var osuBeatmap = OsuParsers.Parser.ParseBeatmap(osuFilePath);

            if (osuBeatmap.GeneralSection.Mode != OsuParsers.Enums.Ruleset.Mania) throw new System.Exception("Not a mania map");

            int keyCount = (int)osuBeatmap.DifficultySection.CircleSize;
            if (keyCount < PolyMesh.MINIMUM_COUNT) throw new System.Exception($"Key count less than {PolyMesh.MINIMUM_COUNT} is not supported");

            Note[] notes = ToPolymaniaNotes(keyCount, osuBeatmap.HitObjects).ToArray();

            //Make it absolute if its relative
            string osuSongPath = Path.GetDirectoryName(Path.GetFullPath(osuFilePath));

            //TODO beatmap.EventsSection.BackgroundImage ?
            Beatmap beatmap = new Beatmap()
            {
                SliceCount = (uint)keyCount,
                SongName = osuBeatmap.MetadataSection.TitleUnicode,
                RomanizedSongName = osuBeatmap.MetadataSection.Title,
                SongPath = Path.Combine(osuSongPath, osuBeatmap.GeneralSection.AudioFilename),
                Notes = notes.ToArray(),
                BackgroundPath = Path.Combine(osuSongPath, osuBeatmap.EventsSection.BackgroundImage),
                DifficultyName = osuBeatmap.MetadataSection.Version,
                AccMod = osuBeatmap.DifficultySection.OverallDifficulty,
                SpeedMod = osuBeatmap.DifficultySection.ApproachRate
            };
            return beatmap;
        }
    }

    public Beatmap()
    {

    }

    static void CopySongToLocal(string externalPath, string localPath)
    {
        AudioClip song = GetAudio(externalPath);
        byte[] wavData = WavUtility.FromAudioClip(song);
        using (FileStream stream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(wavData, 0, wavData.Length);
        }
    }
    static void CopyBackgroundToLocal(string externalPath, string localPath)
    {
        Texture2D tex = Helper.LoadPNG(externalPath);
        byte[] bg = tex.EncodeToPNG();
        using (FileStream stream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(bg, 0, bg.Length);
        }
    }

#if UNITY_EDITOR

    [MenuItem("Beatmap/Import Mania Map")]
    static void ImportManiaMap()
    {
        string loadPath = EditorUtility.OpenFilePanel("Load Mania File", "", "osu");
        if (loadPath == "") return; //Pressed cancel
        Beatmap beatmap = Mania.FromFilePath(loadPath);

        string path = Path.Combine(BeatmapStore.DefaultSongPath, beatmap.SanitizedName);

        //Create the songs directory if it doesnt exist
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (beatmap.SongPath != null)
        {
            string songPath = Path.Combine(path, "song.wav");
            if (!File.Exists(songPath))
            {
                CopySongToLocal(beatmap.SongPath, songPath);
            }
            beatmap.SongPath = songPath;
        }

        if (beatmap.BackgroundPath != null)
        {
            string bgPath = Path.Combine(path, "bg.png");
            if (!File.Exists(bgPath))
            {
                CopyBackgroundToLocal(beatmap.BackgroundPath, bgPath);
            }
            beatmap.BackgroundPath = bgPath;
        }

        BeatmapStore.AddSong(beatmap);
    }

#endif

}