using System;

public enum AccuracyType
{
    None = 0, Amazing = 20, Excellent = 40, Good = 80, Okay = 110, Bad = 160, Horrendous = int.MaxValue
}

public struct Accuracy {
    public AccuracyType type;
    public bool tooLate;

    public Accuracy(AccuracyType type, bool tooLate)
    {
        this.type = type;
        this.tooLate = tooLate;
    }

    public override string ToString()
    {
        return (tooLate ? "late" : "early") + " " + Enum.GetName(typeof(AccuracyType), type);
    }
}
