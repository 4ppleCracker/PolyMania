using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[JsonConverter(typeof(ReplayEventConverter))]
public abstract class ReplayEvent
{
    public Time time;
    public abstract void Execute();
    public virtual void Serialize(JsonWriter writer)
    {
        writer.WritePropertyName("time"); writer.WriteValue(time.Ms);
    }
    public virtual void Deserialize(JObject obj)
    {
        time = new Time(ms: obj["time"].Value<int>());
    }
}
[Serializable]
public class ReplayMousePositionEvent : ReplayEvent
{
    [JsonConverter(typeof(SimpleVector2Converter))]
    public Vector2 position;
    public override void Execute()
    {
        throw new NotImplementedException();
    }
    public override void Serialize(JsonWriter writer)
    {
        base.Serialize(writer);
        writer.WritePropertyName("position"); writer.WriteValue(SimpleVector2Converter.Vector2ToString(position));
    }
    public override void Deserialize(JObject obj)
    {
        base.Deserialize(obj);
        position = SimpleVector2Converter.StringToVector2(obj["position"].Value<string>());
    }
}
[Serializable]
public class ReplayKeyClickEvent : ReplayEvent
{
    public override void Execute()
    {
        throw new NotImplementedException();
    }
}
[Serializable]
public class Replay {
    public List<ReplayEvent> events = new List<ReplayEvent>();
}
