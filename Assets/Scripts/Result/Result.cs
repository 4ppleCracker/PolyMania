using System;

public struct ResultNote
{
    public Accuracy accuracy;
    public static explicit operator ResultNote(Note note)
    {
        ResultNote resultNote = new ResultNote
        {
            accuracy = note.Accuracy
        };
        return resultNote;
    }
}

public class Result
{
    public ResultNote[] resultNotes;
    public int totalAccuracy;
    public int highestCombo;
    public long score;
    public string uuid;
    public DateTime date;
    public Replay replay;

    public int GetCountForAccuracy(AccuracyType type)
    {
        int count = 0;
        foreach(ResultNote note in resultNotes)
        {
            if (note.accuracy.type == type)
                count++;
        }
        return count;
    }
}
