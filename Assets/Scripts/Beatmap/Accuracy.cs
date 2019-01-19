using System;

public enum AccuracyType
{
    Amazing = 150, Excellent = 400, Good = 800, Okay = 1600, Bad = 2600, Horrendous = int.MaxValue, Miss = 0
}

public class Accuracy {
    public AccuracyType type;
    public bool tooLate;

    public Accuracy(AccuracyType type, bool tooLate)
    {
        this.type = type;
        this.tooLate = tooLate;
    }
    public Accuracy()
    {

    }

    /// <returns>value of 0-100 depending on the accuracy</returns>
    public int ToPercent()
    {
        int percent = 0;
        switch(type)
        {
            case AccuracyType.Amazing:
                percent = 100;
                break;
            case AccuracyType.Excellent:
                percent = 90;
                break;
            case AccuracyType.Good:
                percent = 70;
                break;
            case AccuracyType.Okay:
                percent = 50;
                break;
            case AccuracyType.Bad:
                percent = 30;
                break;
            default:
                percent = 0;
                break;
        }
        return percent;
    }

    public override string ToString()
    {
        return (tooLate ? "late" : "early") + " " + Enum.GetName(typeof(AccuracyType), type);
    }
}
