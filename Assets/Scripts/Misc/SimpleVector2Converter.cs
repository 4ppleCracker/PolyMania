using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

public class SimpleVector2Converter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(Vector2);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2 vec = (value as Vector2?).Value;
        writer.WriteValue(Vector2ToString(vec));
    }
    public static string Vector2ToString(Vector2 vec)
    {
        return $"{vec.x.ToString("G")},{vec.y.ToString("G")}";
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string str = reader.ReadAsString();
        return StringToVector2(str);
    }
    public static Vector2 StringToVector2(string str)
    {
        Vector2 vec = new Vector2();
        float[] floats = str.Split(',').Select((string s) => float.Parse(s)).ToArray();
        vec.x = floats[0];
        vec.y = floats[1];
        return vec;
    }
}
