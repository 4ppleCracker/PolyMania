using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Beatmap : ScriptableObject

    ////Loaded data
{
    //Metadata
    public string SongName;

    //Data
    public List<Note> Notes = new List<Note>();
    public List<int> Test = new List<int>();
    public Texture2D BackgroundImage = null;

    //Modifiers
    public int SliceCount;
    public float AccMod;
    public float SpeedMod;
    public int Bpm;

    ////Other data

    public int PlayedNoteCount = 0;

    //Accessors
    public bool AnyNotesLeft => PlayedNoteCount < Notes.Count;

    //Methods
    public Note GetLatestForSlice(int slice)
    {
        for (int i = CurrentlyLoaded.PlayedNoteCount; i < CurrentlyLoaded.Notes.Count; i++)
        {
            Note note = CurrentlyLoaded.Notes[i];
            if (note.slice == slice)
            {
                return note;
            }
        }
        throw new System.Exception();
    }
    public int GetIndexForNote(Note note)
    {
        for (int i = CurrentlyLoaded.PlayedNoteCount; i < CurrentlyLoaded.Notes.Count; i++)
        {
            Note tempNote = CurrentlyLoaded.Notes[i];
            if (tempNote == note)
            {
                return i;
            }
        }
        throw new System.Exception();
    }

    //Loading
    public static Beatmap CurrentlyLoaded { get; private set; }
    public static void Load(Beatmap map)
    {
        map.PlayedNoteCount = 0;
        List<Note> newNotes = new List<Note>();
        foreach(Note note in map.Notes)
        {
            newNotes.Add(new Note(note.time, note.slice));
        }
        map.Notes = newNotes;
        CurrentlyLoaded = map;
        PolyMesh.Instance.Generate(PolyMesh.Instance.Radius, map.SliceCount);

        Debug.Log($"Loaded song {map.SongName}");
    }

    //Required so we can check SliceCount
    public Beatmap(int bpm, int sliceCount, float accMod, float speedMod, string songName, Texture2D background)
    {
        if (sliceCount < PolyMesh.MINIMUM_COUNT)
            throw new System.Exception("sliceCount parameter must be bigger than " + PolyMesh.MINIMUM_COUNT);

        Bpm = bpm;
        SliceCount = sliceCount;
        AccMod = accMod;
        SpeedMod = speedMod;
        SongName = songName;
        BackgroundImage = background;
    }

    public void Reload()
    {
        Load(this);
    }

    //Load a test map by default
    static Beatmap()
    {
        Load(Resources.Load<Beatmap>("TestMap"));
    }
}