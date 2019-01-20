using Newtonsoft.Json;
using System;

public class TimeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(Time);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Time time = (value as Time?).Value;
        writer.WriteValue(time.Ms);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        int ms = reader.ReadAsInt32().Value;
        return new Time(ms);
    }
}

[Serializable]
[JsonConverter(typeof(TimeConverter))]
public struct Time : IComparable<Time>, IEquatable<Time>
{
    public int Ms;
    [JsonIgnore]
    public float Sec => Ms / 1000;

    public Time(int ms)
    {
        Ms = ms;
    }
    public Time(float sec)
    {
        Ms = (int)(sec * 1000);
    }
    public Time(Time time)
    {
        Ms = time.Ms;
    }

    public Time AddMs(int ms)
    {
        return new Time(Ms + ms);
    }
    public Time SubtractMs(int ms)
    {
        return new Time(Ms - ms);
    }

    public bool IsBefore(TimeFrame frame)
    {
        return this < frame.start;
    }
    public bool IsAfter(TimeFrame frame)
    {
        return this > frame.end;
    }
    public bool IsWithin(TimeFrame frame)
    {
        return this <= frame.end && this >= frame.start;
    }

    public int CompareTo(Time obj) => Ms.CompareTo(obj.Ms);
    public bool Equals(Time other) => Ms == other.Ms;
    public override int GetHashCode() => 2073418786 + Ms.GetHashCode();
    public override bool Equals(object obj) => obj is Time && Equals((Time)obj);
    public override string ToString() => Ms + " ms";
    public static bool operator >(Time me, Time other) => me.Ms > other.Ms;
    public static bool operator <(Time me, Time other) => me.Ms < other.Ms;
    public static bool operator ==(Time me, Time other) => me.Ms == other.Ms;
    public static bool operator !=(Time me, Time other) => me.Ms != other.Ms;
    public static bool operator >=(Time me, Time other) => me.Ms >= other.Ms;
    public static bool operator <=(Time me, Time other) => me.Ms <= other.Ms;
    public static Time operator +(Time me, Time other) => me + other.Ms;
    public static Time operator -(Time me, Time other) => me - other.Ms;
    public static Time operator /(Time me, Time other) => me / other.Ms;
    public static Time operator +(Time me, int other) => new Time(ms: me.Ms + other);
    public static Time operator -(Time me, int other) => new Time(ms: me.Ms - other);
    public static Time operator /(Time me, int other) => new Time(ms: me.Ms / other);
}