using System.Collections.Generic;
using UnityEngine;

public class Beatmap

    ////Loaded data
{
    //Metadata
    public string SongName;

    //Data
    public List<Note> Notes;
    public Texture2D BackgroundImage;

    //Modifiers
    public readonly int SliceCount;
    public readonly float AccMod;
    public readonly float SpeedMod;
    public int Bpm;

    ////Other data

    public int PlayedNoteCount;

    //Accessors
    public bool AnyNotesLeft => PlayedNoteCount < Notes.Count;

    //Loading
    public static Beatmap CurrentlyLoaded { get; private set; }
    public static void Load(Beatmap map)
    {
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

        //Default values
        PlayedNoteCount = 0;
        Notes = null;
    }

    //Load a dummy map by default
    static Beatmap()
    {
        Load(new Beatmap(bpm: 120, sliceCount: PolyMesh.Instance.Count, accMod: 8, speedMod: 5, songName: "120 bpm drum", background: Resources.Load<Texture2D>("Textures/testBackground"))
        {
            Notes = new List<Note>
            {
                new Note(time: new Time(ms: 2062)                                   , slice: 0),
                new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(1, 120, 4), slice: 1),
                new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(2, 120, 4), slice: 2),
                new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(3, 120, 4), slice: 3),
            }
        });
    }
}
