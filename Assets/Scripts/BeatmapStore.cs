using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class BeatmapStoreInfo
{
    public string SongName;
    public string RomanizedSongName;
    public string DifficultyName;
    public string SongPath;
    public string MapPath;
    public string BackgroundPath;
    public string uuid;

    public Beatmap GetBeatmap() {
        return BeatmapStore.DeserializeBeatmap(MapPath);
    }
}

static class BeatmapStore
{
    public const string DefaultSongPath = "Songs";
    public static List<BeatmapStoreInfo> Beatmaps;

    public static Beatmap DeserializeBeatmap(string fileName)
    {
        Beatmap beatmap = new Beatmap();

        using (StreamReader stream = new StreamReader(fileName))
        using (JsonReader reader = new JsonTextReader(stream))
        {
            reader.Read(); //Start
            {
                reader.Read(); //Version
                beatmap.version = reader.ReadAsString();

                reader.Read(); //Metadata
                {
                    reader.Read(); //Song name
                    beatmap.SongName = reader.ReadAsString();

                    reader.Read(); //Romanized song name
                    beatmap.RomanizedSongName = reader.ReadAsString(); 

                    reader.Read(); //Dificulty name
                    beatmap.DifficultyName = reader.ReadAsString(); 
                }
                reader.Read(); //Modifierdata
                {
                    reader.Read(); //Speed mod
                    beatmap.SpeedMod = (float)reader.ReadAsDouble(); 

                    reader.Read(); //Acc mod
                    beatmap.AccMod = (float)reader.ReadAsDouble();

                    reader.Read(); //Slice count
                    beatmap.SliceCount = (uint)reader.ReadAsInt32();
                }
                reader.Read(); //Filedata
                {
                    reader.Read(); //Song path
                    beatmap.SongPath = reader.ReadAsString();

                    reader.Read(); //Background path
                    beatmap.BackgroundPath = reader.ReadAsString();
                }
                reader.Read(); //Notedata
                {
                    List<Note> notes = new List<Note>();
                    reader.Read(); //Notes
                    reader.Read(); //Start of array
                    //Read start of object or end array and make sure its not an end array, if its not, parse the note
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        Note note = new Note();
                        {
                            reader.Read(); //Time
                            note.time = new Time(ms: (int)reader.ReadAsInt32());

                            reader.Read(); //Slice
                            note.slice = (uint)reader.ReadAsInt32();
                        }
                        reader.Read(); //End of object
                        notes.Add(note);
                    }
                    reader.Read(); //End of array
                    beatmap.Notes = notes.ToArray();
                }
            }
            reader.Read(); //End
        }

        return beatmap;
    }
    public static void SerializeBeatmap(Beatmap map, string fileName)
    {
        StringBuilder sb = new StringBuilder();
        using (StringWriter stream = new StringWriter(sb))
        using (JsonWriter writer = new JsonTextWriter(stream))
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();
            {
                writer.WritePropertyName("version"); writer.WriteValue("1.0");
                writer.WriteWhitespace("\n");
                writer.WriteComment("Metadata");
                {
                    writer.WritePropertyName("SongName"); writer.WriteValue(map.SongName);
                    writer.WritePropertyName("RomanizedSongName"); writer.WriteValue(map.RomanizedSongName);
                    writer.WritePropertyName("DifficultyName"); writer.WriteValue(map.DifficultyName);
                }
                writer.WriteWhitespace("\n");
                writer.WriteComment("Modifierdata");
                {
                    writer.WritePropertyName("SpeedMod"); writer.WriteValue(map.SpeedMod);
                    writer.WritePropertyName("AccMod"); writer.WriteValue(map.AccMod);
                    writer.WritePropertyName("SliceCount"); writer.WriteValue(map.SliceCount);
                }
                writer.WriteWhitespace("\n");
                writer.WriteComment("Filedata");
                {
                    writer.WritePropertyName("SongPath"); writer.WriteValue(map.SongPath);
                    writer.WritePropertyName("BackgroundPath"); writer.WriteValue(map.BackgroundPath);
                }
                writer.WriteWhitespace("\n");
                writer.WriteComment("Notedata");
                {
                    writer.WritePropertyName("Notes");
                    writer.WriteStartArray();
                    foreach (Note note in map.Notes) {
                        writer.WriteStartObject();
                        {
                            writer.WritePropertyName("time"); writer.WriteValue(note.time.Ms);
                            writer.WritePropertyName("slice"); writer.WriteValue(note.slice);
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }
        File.WriteAllText(fileName, sb.ToString());
    }

    public static void LoadSong(string songPath)
    {
        foreach(string file in Directory.EnumerateFiles(songPath))
        {
            if(file.EndsWith(".pmb"))
            {
                Beatmap beatmap = DeserializeBeatmap(file);
                BeatmapStoreInfo beatmapStoreInfo = new BeatmapStoreInfo() {
                    MapPath = file,
                    SongPath = beatmap.SongPath,
                    SongName = beatmap.SongName,
                    RomanizedSongName = beatmap.RomanizedSongName,
                    DifficultyName = beatmap.DifficultyName,
                    BackgroundPath = beatmap.BackgroundPath,
                    uuid = beatmap.GetUUID()
                };
                if (Beatmaps == null)
                    Beatmaps = new List<BeatmapStoreInfo>();
                Beatmaps.Add(beatmapStoreInfo);
            }
        }
    }
    public static void AddSong(Beatmap map)
    {
        string targetDirectory = Path.Combine(DefaultSongPath, map.SanitizedName) + "/";
        if(!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);
        string file = Path.Combine(targetDirectory, Helper.SanitizeString(map.DifficultyName) + ".pmb");
        SerializeBeatmap(map, file);
    }
    public static void LoadAll(string basePath=DefaultSongPath)
    {
        var songPaths = Directory.EnumerateDirectories(basePath);
        Beatmaps = new List<BeatmapStoreInfo>(songPaths.Count());
        foreach (string songPath in songPaths)
        {
            LoadSong(songPath);
        }
    }
};