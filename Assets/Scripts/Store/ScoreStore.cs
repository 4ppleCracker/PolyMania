using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

static class ScoreStore
{
    public static Dictionary<string, SortedList<long, Result[]>> Scores = null;
    public const string FilePath = "score.xml";

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

        using (XmlReader reader = XmlReader.Create(FilePath))
        {
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (reader.Name == "score" && reader.NodeType == XmlNodeType.Element)
                {
                    string data = reader.ReadInnerXml();

                    string decryptedScoreXml = Caesar(data, -41);

                    XmlSerializer serializer = new XmlSerializer(typeof(Result));
                    using (StringReader textReader = new StringReader(decryptedScoreXml))
                    {
                        Result score = (Result)serializer.Deserialize(textReader);

                        SetScoresOn(score.uuid, score);
                    }
                }
                else reader.Read();
            }
        }
    }
    public static void AddOfflineScore(Result score)
    {
        XDocument doc = null;
        if (!File.Exists(FilePath))
        {
            doc = new XDocument(new XElement("root"));
        }
        else
        {
            doc = XDocument.Load(FilePath);
        }

        string encryptedScoreXml = Caesar(SerializeToString(score), 41);
        doc.Root.Add(new XElement("score", encryptedScoreXml));

        XmlWriterSettings settings = new XmlWriterSettings
        {
            CheckCharacters = false,
            Indent = true
        };
        using (XmlWriter writer = XmlWriter.Create(FilePath, settings))
        {
            doc.Save(writer);
        }
    }

    public static string SerializeToString<T>(T obj)
    {
        var emptyNamespaces = new XmlSerializerNamespaces(new XmlQualifiedName[] { XmlQualifiedName.Empty });
        var serializer = new XmlSerializer(typeof(T));
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
        {
            serializer.Serialize(xmlWriter, obj, emptyNamespaces);
            return stringWriter.ToString();
        }
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