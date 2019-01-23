using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class BeatmapSerializations
{
    public class UnsupportedVersionException : Exception
    {
        public override string Message => "This beatmap version is unsupported";
    }
    public class InvalidBeatmapException : Exception
    {
        public override string Message => "This beatmap format is invalid";
    }

    static readonly IBeatmapDeserializer m_latestDeserializer = new Version1_1();

    public static Func<Beatmap> GetSerializer(string fileName, bool upgradeMap=true)
    {
        Func<Beatmap> deserializer = null;
        string version = null;
        using (StreamReader stream = new StreamReader(fileName))
        {
            string text = stream.ReadToEnd();
            //Test Json parsers
            {
                JObject jObject;
                if (Helper.TryParseJObject(text, out jObject))
                {
                    version = jObject["version"].Value<string>();
                    if (version == "1.1")
                    {
                        deserializer = () => Version1_1.Deserialize(jObject);
                        goto end;
                    }
                    else
                    {
                        Debug.Log($"Version {version} is not supported");
                        goto end;
                    }
                }
            }
        }
    end:
        if (upgradeMap && deserializer != null && version != null && m_latestDeserializer != null && version != m_latestDeserializer.Version)
        {
            Debug.Log($"Upgrading map from version {version} to {m_latestDeserializer.Version}");
            Beatmap map = deserializer.Invoke();
            string file = BeatmapStore.GetFileNameForMap(BeatmapStore.GetFolderForMap(map), map);
            SerializeBeatmap(map, file);
            deserializer = GetSerializer(file, false);
        }
        return deserializer;
    }

    interface IBeatmapDeserializer
    {
        string Version { get; }
    }

    private class Version1_1 : IBeatmapDeserializer
    {
        public string Version => "1.1";
        public static Beatmap Deserialize(JObject jObject)
        {
            Beatmap beatmap = new Beatmap();
            {
                //Metadata
                beatmap.SongName = (string)jObject["SongName"];
                beatmap.RomanizedSongName = (string)jObject["RomanizedSongName"];
                beatmap.DifficultyName = (string)jObject["DifficultyName"];
                beatmap.Author = (string)jObject["Author"];
                beatmap.Artist = (string)jObject["Artist"];
                beatmap.RomanizedArtist = (string)jObject["RomanizedArtist"];

                //Modifierdata
                beatmap.SpeedMod = (float)jObject["SpeedMod"];
                beatmap.AccMod = (float)jObject["AccMod"];
                beatmap.SliceCount = (uint)jObject["SliceCount"];

                //Filedata
                beatmap.BackgroundPath = (string)jObject["BackgroundPath"];
                beatmap.SongPath = (string)jObject["SongPath"];

                JArray Jnotes = (JArray)jObject["Notes"];
                beatmap.Notes = new Note[Jnotes.Count];
                for (int i = 0; i < Jnotes.Count; i++)
                {
                    JToken token = Jnotes[i];
                    beatmap.Notes[i] = new Note()
                    {
                        time = new Time(ms: (int)token["time"]),
                        slice = (uint)token["slice"]
                    };
                }
            }
            return beatmap;
        }
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
                writer.WritePropertyName("version"); writer.WriteValue(m_latestDeserializer.Version);
                writer.WriteWhitespace("\n");
                writer.WriteComment("Metadata");
                {
                    writer.WritePropertyName("SongName"); writer.WriteValue(map.SongName);
                    writer.WritePropertyName("RomanizedSongName"); writer.WriteValue(map.RomanizedSongName);
                    writer.WritePropertyName("DifficultyName"); writer.WriteValue(map.DifficultyName);
                    writer.WritePropertyName("Author"); writer.WriteValue(map.Author);
                    writer.WritePropertyName("Artist"); writer.WriteValue(map.Artist);
                    writer.WritePropertyName("RomanizedArtist"); writer.WriteValue(map.RomanizedArtist);
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
                    foreach (Note note in map.Notes)
                    {
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
}