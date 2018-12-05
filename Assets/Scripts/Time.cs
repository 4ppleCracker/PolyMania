using System;

[Serializable]
public struct Time : IComparable<Time>, IEquatable<Time>
{
    public int Ms;
    public float Sec => Ms / 1000;

    public Time(int ms)
    {
        Ms = ms;
    }
    public Time(float sec)
    {
        Ms = (int)(sec * 1000);
    }

    public int CompareTo(Time obj) => Ms.CompareTo(obj.Ms);
    public bool Equals(Time other) => Ms == other.Ms;
    public override int GetHashCode() => 2073418786 + Ms.GetHashCode();
    public override bool Equals(object obj) => obj is Time && Equals((Time)obj);
    public override string ToString() => Ms + " ms";
    public static implicit operator Time(float sec) => new Time(sec);
    public static implicit operator Time(int ms) => new Time(ms);
    public static bool operator >(Time me, Time other) => me.Ms > other.Ms;
    public static bool operator <(Time me, Time other) => me.Ms < other.Ms;
    public static bool operator ==(Time me, Time other) => me.Ms == other.Ms;
    public static bool operator !=(Time me, Time other) => me.Ms != other.Ms;
    public static bool operator >=(Time me, Time other) => me.Ms >= other.Ms;
    public static bool operator <=(Time me, Time other) => me.Ms <= other.Ms;
    public static Time operator +(Time me, Time other) => me.Ms + other.Ms;
    public static Time operator -(Time me, Time other) => me.Ms - other.Ms;
    public static Time operator /(Time me, Time other) => me.Ms / other.Ms;
}