using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class BeatmapStoreInfo
{
    public string SongName;
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
        using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            return (Beatmap)(new XmlSerializer(typeof(Beatmap)).Deserialize(stream));
        }
    }
    public static void SerializeBeatmap(Beatmap map, string fileName)
    {
        using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            new XmlSerializer(typeof(Beatmap)).Serialize(stream, map);
        }
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