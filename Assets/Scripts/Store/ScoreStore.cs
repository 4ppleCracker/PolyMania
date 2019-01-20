using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

static class ScoreStore
{
    public static Dictionary<string, SortedList<long, Result[]>> Scores = null;
    public const string FilePath = "score.json";

    private static void SetScoresOn(string index, Result score)
    {
        SortedList<long, Result[]> list;
        if (Scores.ContainsKey(index))
            list = Scores[index];
        else
            Scores.Add(index, list = new SortedList<long, Result[]>());
        Result[] scores = null;
        if (list.TryGetValue(score.score, out scores))
        {
            Result[] newScores = new Result[scores.Length + 1];
            Array.Copy(scores, newScores, scores.Length);
            newScores[newScores.Length - 1] = score;
            list[score.score] = newScores;
        }
        else
        {
            list.Add(score.score, new[] { score });
        }
    }

    public static void LoadOffline()
    {
        if (!File.Exists(FilePath)) return;

        Scores = new Dictionary<string, SortedList<long, Result[]>>();

        string json = File.ReadAllText(FilePath);

        List<Result> results = JsonConvert.DeserializeObject<List<Result>>(json);

        foreach (Result result in results) {
            SetScoresOn(result.uuid, result);
        }
    }
    public static void AddOfflineScore(Result score)
    {
        JArray obj = null;
        if (!File.Exists(FilePath))
        {
            obj = new JArray();
        }
        else
        {
            string json = File.ReadAllText(FilePath);
            obj = JArray.Parse(json);
        }

        string unencryptedScoreJson = JsonConvert.SerializeObject(score, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        string encryptedScoreJson = Caesar(unencryptedScoreJson, 41);
        obj.Add(JObject.Parse(unencryptedScoreJson));

        File.WriteAllText(FilePath, obj.ToString(Newtonsoft.Json.Formatting.Indented));
    }

    public static short CeasarKey = 41;

    public static string Caesar(this string source, short shift)
    {
        var maxChar = Convert.ToInt32(char.MaxValue);
        var minChar = Convert.ToInt32(char.MinValue);

        var buffer = source.ToCharArray();

        for (var i = 0; i < buffer.Length; i++)
        {
            var shifted = Convert.ToInt32(buffer[i]) + shift;

            if (shifted > maxChar)
            {
                shifted -= maxChar;
            }
            else if (shifted < minChar)
            {
                shifted += maxChar;
            }

            buffer[i] = Convert.ToChar(shifted);
        }

        return new string(buffer);
    }
}