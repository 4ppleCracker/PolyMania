using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

class BeatmapStoreInfo
{
    public string SongPath;
    public string MapPath;
}

static class BeatmapStore
{
    public const string SongPath = "Songs";
    public static BeatmapStoreInfo[] Beatmaps;

    private static Beatmap DeserializeBeatmap(string fileName)
    {
        using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.None))
        {
            return (Beatmap)(new XmlSerializer(typeof(Beatmap)).Deserialize(stream));
        }
    }
    private static void SerializeBeatmap(Beatmap map, string fileName)
    {
        if (new DirectoryInfo(map.SongPath).Parent.FullName != Path.Combine(Directory.GetCurrentDirectory(), SongPath))
        {
            //Move song to local if its not

        }

        using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            new XmlSerializer(typeof(Beatmap)).Serialize(stream, map);
        }
    }

    public static void LoadSong(string songPath, bool resizeStorage=true)
    {
        int i = 0;
        foreach(string file in Directory.EnumerateFiles(songPath))
        {
            if(file.EndsWith(".pmb"))
            {
                Beatmap beatmap = DeserializeBeatmap(file);
                BeatmapStoreInfo beatmapStoreInfo = new BeatmapStoreInfo() { MapPath = file, SongPath = beatmap.SongPath };
                if (resizeStorage)
                {
                    BeatmapStoreInfo[] newBeatmaps = new BeatmapStoreInfo[(Beatmaps?.Length ?? 0) + 1];
                    if (Beatmaps != null)
                    {
                        Array.Copy(Beatmaps, newBeatmaps, Beatmaps.Length);
                    }
                    newBeatmaps[newBeatmaps.Length] = beatmapStoreInfo;
                    Beatmaps = newBeatmaps;
                }
                else
                {
                    if (Beatmaps == null)
                        Beatmaps = new BeatmapStoreInfo[1];
                    Beatmaps[i] = beatmapStoreInfo;
                }
                i++;
            }
        }
    }
    public static void AddSong(Beatmap map)
    {
        string targetDirectory = Path.Combine(SongPath, map.SongName) + "/";
        if(!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);
        string file = Path.Combine(targetDirectory, map.SongName + "[" + map.DifficultyName + "]" + ".pmb");
        SerializeBeatmap(map, file);
    }
    public static void LoadAll(string basePath=SongPath)
    {
        var songPaths = Directory.EnumerateDirectories(basePath);
        Beatmaps = new BeatmapStoreInfo[songPaths.Count()];
        foreach (string songPath in songPaths)
        {
            LoadSong(songPath, false);
        }
    }
};