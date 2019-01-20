using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class TimeFrame
{
    public Time start, end;

    public TimeFrame(Time start, Time end)
    {
        this.start = start;
        this.end = end;
    }
}

[Serializable]
public struct Note
{
    //Static Data
    public Time time;
    public uint slice;

    //Dynamic Data
    [NonSerialized, XmlIgnore]
    public bool clicked;
    [NonSerialized, XmlIgnore]
    public bool generated;
    [NonSerialized, XmlIgnore]
    public int trueAccuracy;
    [NonSerialized, XmlIgnore]
    private Accuracy m_accuracy;
    [XmlIgnore]
    public Accuracy Accuracy {
        get {
            if (m_accuracy != null)
                return m_accuracy;
            int acc = Math.Abs(trueAccuracy);
            AccuracyType type = AccuracyType.Horrendous;
            foreach (AccuracyType val in Enum.GetValues(typeof(AccuracyType)))
            {
                if (acc < ((int)val / Beatmap.CurrentlyLoaded.AccMod))
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
    [XmlIgnore]
    public TimeFrame HitTimeFrame {
        get {
            return new TimeFrame(time.SubtractMs((int)NotesController.AllowedTimeToClick), time.AddMs((int)NotesController.TimeToMiss));
        }
    }

    //Accessors
    public Time TimeToClick => time - Conductor.Instance.Position;

    public override string ToString()
    {
        return $"Time = {time}, slice = {slice}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Note))
            return false;

        var note = (Note)obj;
        return time.Equals(note.time) &&
               slice == note.slice;
    }

    public string GetUUID()
    {
        return time.Ms.ToString() + slice.ToString();
    }

    public override int GetHashCode()
    {
        var hashCode = 750083029;
        hashCode = hashCode * -1521134295 + EqualityComparer<Time>.Default.GetHashCode(time);
        hashCode = hashCode * -1521134295 + slice.GetHashCode();
        return hashCode;
    }

    public Note(Time time, uint slice)
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
