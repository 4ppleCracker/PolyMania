using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class Beatmap

    ////Loaded data
{
    //Metadata
    public string SongName;
    public string DifficultyName;

    //Data
    public Note[] Notes;
    [XmlIgnore]
    public Texture2D BackgroundImage = null;
    public string BackgroundPath;
    [XmlIgnore]
    public AudioClip Song;
    public string SongPath;

    //Modifiers
    public uint SliceCount;
    public float AccMod;
    public float SpeedMod;
    public int Bpm;

    //Accessors
    //FUTURE ME, DO NOT CHANGE THIS, ITS CORRECT, 3 <= 3 is true, meaning there are notes left when there arent
    public bool AnyNotesLeft => PlayedNotes.Count() < Notes.Length;

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
        throw new System.Exception();
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
        throw new System.Exception("Couldnt find note " + note);
    }
    public IEnumerable<Note> PlayedNotes => Notes.Where(note => note.clicked);

    //Loading
    public static Beatmap CurrentlyLoaded { get; private set; }
    public static void Load(Beatmap map)
    {
        List<Note> newNotes = new List<Note>();
        foreach(Note note in map.Notes)
        {
            newNotes.Add(new Note(note.time, note.slice));
        }
        map.Notes = newNotes.ToArray();
        CurrentlyLoaded = map;

        Debug.Log($"Loaded song {map.SongName}");
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
        static float bytesToFloat(byte firstByte, byte secondByte)
        {
            // convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);
            // convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }

        static int bytesToInt(byte[] bytes, int offset = 0)
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
            Frequency = bytesToInt(wav, 24);

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
                LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                if (ChannelCount == 2)
                {
                    RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
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
    private static AudioClip GetMp3Audio(string name, byte[] data)
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
    public static AudioClip GetAudio(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        FileInfo fileInfo = new FileInfo(path);
        switch (fileInfo.Extension)
        {
            case ".mp3":
            {
                return GetMp3Audio(fileInfo.Name, data);
            }
            case ".wav":
                return WavUtility.ToAudioClip(data);
            default:
            {
                Debug.Log(fileInfo.Extension + " file type is not yet supported");
                return null;
            }
        }
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }

    public static Beatmap FromMania(string osuFilePath)
    {
        var osuBeatmap = OsuParsers.Parser.ParseBeatmap(osuFilePath);

        if (osuBeatmap.GeneralSection.Mode != OsuParsers.Enums.Ruleset.Mania) throw new System.Exception("Not a mania map");

        uint sliceCount = (uint)osuBeatmap.DifficultySection.CircleSize;

        if (sliceCount < PolyMesh.MINIMUM_COUNT) throw new System.Exception($"Key count less than {PolyMesh.MINIMUM_COUNT} is not supported");

        uint sliceWidth = 512 / sliceCount;
        List<Note> notes = new List<Note>();
        foreach (var hitObject in osuBeatmap.HitObjects)
        {
            //TODO For the 7K + 1 mode, the column index is 1 + x / (512 / 7), leaving the column 0 for the specials.
            uint slice = (uint)(hitObject.Position.X / sliceWidth);
            int time = hitObject.StartTime;

            notes.Add(new Note(new Time(ms: time), slice));
        }

        //Make it absolute if its relative
        osuFilePath = new FileInfo(osuFilePath).Directory.FullName;

        //TODO beatmap.EventsSection.BackgroundImage ?
        Beatmap beatmap = new Beatmap() {
            SliceCount = sliceCount,
            SongName = osuBeatmap.MetadataSection.Title,
            SongPath = Path.Combine(osuFilePath, osuBeatmap.GeneralSection.AudioFilename),
            Notes = notes.ToArray(),
            BackgroundPath = Path.Combine(osuFilePath, osuBeatmap.EventsSection.BackgroundImage),
            DifficultyName = osuBeatmap.MetadataSection.Version,
            AccMod = osuBeatmap.DifficultySection.OverallDifficulty,
            SpeedMod = osuBeatmap.DifficultySection.ApproachRate
        };
        beatmap.Song = GetAudio(beatmap.SongPath);
        beatmap.BackgroundImage = LoadPNG(beatmap.BackgroundPath);
        return beatmap;
    }

    public Beatmap()
    {

    }

    [MenuItem("Beatmap/Import Mania Map")]
    static void ImportManiaMap()
    {
        string loadPath = EditorUtility.OpenFilePanel("Load Mania File", "", "osu");
        if (loadPath == "") return; //Pressed cancel
        Beatmap beatmap = FromMania(loadPath);

        string path = Path.Combine(BeatmapStore.DefaultSongPath, beatmap.SongName);

        //Create the songs directory if it doesnt exist
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string songPath = Path.Combine(path, "song.wav");
        if (!File.Exists(songPath))
        {
            byte[] wavData = WavUtility.FromAudioClip(beatmap.Song);
            using(FileStream stream = new FileStream(songPath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(wavData, 0, wavData.Length);
            }
        }
        beatmap.SongPath = songPath;

        if (beatmap.BackgroundImage != null)
        {
            string bgPath = Path.Combine(path, "bg.png");
            if (!File.Exists(bgPath))
            {
                byte[] bg = beatmap.BackgroundImage.EncodeToPNG();
                using (FileStream stream = new FileStream(bgPath, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(bg, 0, bg.Length);
                }
            }
            beatmap.BackgroundPath = bgPath;
        }

        BeatmapStore.AddSong(beatmap);
    }
}