﻿using System;

[Serializable]
public struct Note
{
    //Static Data
    public Time time;
    public int slice;

    //Dynamic Data
    [NonSerialized]
    public bool clicked;
    [NonSerialized]
    public bool generated;
    [NonSerialized]
    public int trueAccuracy;
    [NonSerialized]
    private Accuracy m_accuracy;
    public Accuracy Accuracy {
        get {
            if (m_accuracy != null)
                return m_accuracy;
            int acc = Math.Abs(trueAccuracy);
            AccuracyType type = AccuracyType.Horrendous;
            foreach (AccuracyType val in Enum.GetValues(typeof(AccuracyType)))
            {
                if (acc < (int)val)
                {
                    type = val;
                    break;
                }
            }
            return m_accuracy = new Accuracy(type, trueAccuracy < 0);
        }
        set {
            m_accuracy = value;
        }
    }

    //Accessors
    public Time TimeToClick => time - Conductor.Instance.Position;

    public override string ToString()
    {
        return $"Time = {time}, slice = {slice}, clicked = {clicked}, generated = {generated}, accuracy = {trueAccuracy}";
    }

    public Note(Time time, int slice)
    {
        this.time = time;
        this.slice = slice;
        clicked = false;
        generated = false;
        trueAccuracy = -1;
        m_accuracy = null;
    }

    public static bool operator==(Note me, Note other)
    {
        return me.slice == other.slice && me.time == other.time;
    }
    public static bool operator !=(Note me, Note other)
    {
        return !(me == other);
    }
}
