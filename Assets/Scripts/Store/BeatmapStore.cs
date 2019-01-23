using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

public static class BeatmapStore
{
    public static string DefaultSongPath =>
        Path.Combine(new string[] {
#if UNITY_ANDROID || UNITY_IOS
            Application.persistentDataPath,
#endif
            "Songs",
        });
    public static List<BeatmapStoreInfo> Beatmaps;

    public static Beatmap DeserializeBeatmap(string fileName)
    {
        return BeatmapSerializations.GetSerializer(fileName)?.Invoke();
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
        string targetDirectory = GetFolderForMap(map);
        if(!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);
        BeatmapSerializations.SerializeBeatmap(map, GetFileNameForMap(targetDirectory, map));
    }
    public static void LoadAll(string basePath=null)
    {
        if (basePath == null) basePath = DefaultSongPath;
        if (!Directory.Exists(basePath))
        {
            Debug.Log("Didn't find a songs folder, creating one");
            Directory.CreateDirectory(basePath);
        }
        else
        {
            var songPaths = Directory.EnumerateDirectories(basePath);
            Beatmaps = new List<BeatmapStoreInfo>(songPaths.Count());
            foreach (string songPath in songPaths)
            {
                LoadSong(songPath);
            }
        }
    }
    public static string GetFileNameForMap(string targetDirectory, Beatmap map) =>
        Path.Combine(targetDirectory, Helper.SanitizeString(map.DifficultyName) + ".pmb");
    public static string GetFolderForMap(Beatmap map) =>
        Path.Combine(DefaultSongPath, map.SanitizedName) + "/";
};