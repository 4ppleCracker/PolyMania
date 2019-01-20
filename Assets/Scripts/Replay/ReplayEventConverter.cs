using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class ReplayEventConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(ReplayEvent);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        ReplayEvent replayEvent = value as ReplayEvent;
        writer.WriteStartObject();
        {
            writer.WritePropertyName("type"); writer.WriteValue(value.GetType().ToString());
            writer.WritePropertyName("data"); writer.WriteStartObject();
            {
                replayEvent.Serialize(writer);
            }
            writer.WriteEndObject();
        }
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject obj = JToken.ReadFrom(reader) as JObject;
        Type type = Type.GetType(obj["type"].Value<string>());
        ReplayEvent replayEvent = Activator.CreateInstance(type) as ReplayEvent;
        replayEvent.Deserialize(obj["data"] as JObject);
        return replayEvent;
    }
}
