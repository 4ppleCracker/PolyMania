using System;

public struct Time : IComparable<Time>, IEquatable<Time>
{
    public float Ms => Sec * 1000;
    public float Sec;

    public Time(float sec)
    {
        Sec = sec;
    }

    public int CompareTo(Time obj) => Sec.CompareTo(obj.Sec);
    public bool Equals(Time other) => Sec == other.Sec;
    public override int GetHashCode() => 2073418786 + Sec.GetHashCode();
    public override bool Equals(object obj) => obj is Time && Equals((Time)obj);
    public override string ToString() => Sec + " seconds";
    public static implicit operator Time(int sec) => new Time(sec);
    public static bool operator >(Time me, Time other) => me.Sec > other.Sec;
    public static bool operator <(Time me, Time other) => me.Sec < other.Sec;
    public static bool operator ==(Time me, Time other) => me.Sec == other.Sec;
    public static bool operator !=(Time me, Time other) => me.Sec != other.Sec;
    public static bool operator >=(Time me, Time other) => me.Sec >= other.Sec;
    public static bool operator <=(Time me, Time other) => me.Sec <= other.Sec;
}
