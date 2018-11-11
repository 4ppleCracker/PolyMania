using System;

public struct Note
{
    //Data
    public Time time;
    public int slice;
    public bool clicked;
    public bool generated;
    public bool missed;
    public int msAccuracy;
    private Accuracy m_accuracy;
    public Accuracy Accuracy {
        get {
            if (m_accuracy.type != AccuracyType.None)
                return m_accuracy;
            int acc = Math.Abs(msAccuracy);
            AccuracyType type = AccuracyType.Horrendous;
            foreach (AccuracyType val in Enum.GetValues(typeof(AccuracyType)))
            {
                if (acc < (int)val)
                {
                    type = val;
                    break;
                }
            }
            return m_accuracy = new Accuracy(type, acc > 0);
        }
    }

    //Accessors
    public Time TimeToClick => time - Conductor.Instance.Position;

    public override string ToString()
    {
        return $"Time = {time}, slice = {slice}, clicked = {clicked}, generated = {generated}, missed = {missed}, accuracy = {msAccuracy}";
    }

    public Note(Time time, int slice)
    {
        this.time = time;
        this.slice = slice;
        clicked = false;
        generated = false;
        missed = false;
        msAccuracy = -1;
        m_accuracy = new Accuracy(AccuracyType.None, false);
    }
}
