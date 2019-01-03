using Newtonsoft.Json.Linq;
using System;
using System.IO;

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

    public static BeatmapSerializer GetSerializer(string fileName)
    {
        using (StreamReader stream = new StreamReader(fileName))
        {
            string text = stream.ReadToEnd();
            //Test Json parsers
            {
                JObject jObject;
                if (Helper.TryParseJObject(text, out jObject))
                {
                    string version = (string)jObject["version"];
                    if (version == "1.1")
                    {
                        return new Version1_1()
                        {
                            jObject = jObject
                        };
                    }
                    else
                    {
                        throw new UnsupportedVersionException();
                    }
                }
            }
        }
        throw new InvalidBeatmapException();
    }

    public abstract class BeatmapSerializer
    {
        public abstract Beatmap Deserialize();
    }

    private class Version1_1 : BeatmapSerializer
    {
        internal JObject jObject;
        public override Beatmap Deserialize()
        {
            Beatmap beatmap = new Beatmap();
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
}