using System;

public enum AccuracyType
{
    None = 0, Fantastic = 20, Excellent = 50, Good = 70, Okay = 80, Bad = 100, Horrendous = int.MaxValue
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
