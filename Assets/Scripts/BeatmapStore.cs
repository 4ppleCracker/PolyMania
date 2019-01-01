using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
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
    class UnsupportedVersionException : Exception
    {
        public override string Message => "This beatmap version is unsupported";
    }
    class InvalidBeatmapException : Exception
    {
        public override string Message => "This beatmap format is invalid";
    }

    public const string DefaultSongPath = "Songs";
    public static List<BeatmapStoreInfo> Beatmaps;

    public static Beatmap DeserializeBeatmap(string fileName)
    {
        Beatmap beatmap = new Beatmap();

        using (StreamReader stream = new StreamReader(fileName))
        {
            try
            {
                JObject jObject = JObject.Parse(stream.ReadToEnd());
                beatmap.version = (string)jObject["version"];
                if (beatmap.version == "1.1")
                {
                    //Metadata
                    beatmap.SongName = (string)jObject["SongName"];
                    beatmap.RomanizedSongName = (string)jObject["RomanizedSongName"];
                    beatmap.DifficultyName = (string)jObject["DifficultyName"];

                    //Modifierdata
                    beatmap.SpeedMod = (float)jObject["SpeedMod"];
                    beatmap.AccMod = (float)jObject["AccMod"];
                    beatmap.SliceCount = (uint)jObject["SliceCount"];

                    //Filedata
                    beatmap.BackgroundPath = (string)jObject["BackgroundPath"];
                    beatmap.SongPath = (string)jObject["SongPath"];

                    JArray Jnotes = (JArray)jObject["Notes"];
                    beatmap.Notes = new Note[Jnotes.Count];
                    for(int i = 0; i < Jnotes.Count; i++)
                    {
                        JToken token = Jnotes[i];
                        Note note = new Note();
                        note.time = new Time(ms: (int)token["time"]);
                        note.slice = (uint)token["slice"];
                        beatmap.Notes[i] = note;
                    }
                }
                else
                {
                    throw new UnsupportedVersionException();
                }
            }
            catch(UnsupportedVersionException ex)
            {
                throw ex;
            }
            catch
            {
                throw new InvalidBeatmapException();
            }
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
                writer.WritePropertyName("version"); writer.WriteValue("1.1");
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
                if(beatmap == null)
                {
                    Debug.Log("Failed to load map " + file);
                    continue;
                }
                beatmap.Fix();
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
        if (!Directory.Exists(basePath)) return;
        var songPaths = Directory.EnumerateDirectories(basePath);
        Beatmaps = new List<BeatmapStoreInfo>(songPaths.Count());
        foreach (string songPath in songPaths)
        {
            LoadSong(songPath);
        }
    }
};