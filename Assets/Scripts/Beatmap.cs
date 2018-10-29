using System.Collections.Generic;

public class Beatmap
{
    public Queue<Note> Notes;
    public bool AnyNotesLeft => Notes.Count > 0;
    public Note NextNote => Notes.Peek();
    public static Beatmap CurrentlyLoaded = new Beatmap()
    {
        Notes = new Queue<Note>(new Note[] 
        {
            new Note(100000, 1)
        })
    };
}
