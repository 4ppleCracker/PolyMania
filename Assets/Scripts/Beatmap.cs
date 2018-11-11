using System.Collections.Generic;
using UnityEngine;

public class Beatmap
{
    //Metadata
    public string SongName;

    //Data
    public List<Note> Notes;
    public int PlayedNoteCount;

    //Modifiers
    public readonly int SliceCount;
    public readonly float AccMod;
    public readonly float SpeedMod;
    public int Bpm;

    //Accessors
    public bool AnyNotesLeft => PlayedNoteCount <= Notes.Count;

    //Loading
    public static Beatmap CurrentlyLoaded { get; private set; }
    public static void Load(Beatmap map)
    {
        CurrentlyLoaded = map;
        PolyMesh.Instance.Generate(PolyMesh.Instance.Radius, map.SliceCount);

        Debug.Log($"Loaded song {map.SongName}");
    }

    //Required so we can check SliceCount
    public Beatmap(int bpm, int sliceCount, float accMod, float speedMod, string songName)
    {
        if (sliceCount < PolyMesh.MINIMUM_COUNT)
            throw new System.Exception("sliceCount parameter must be bigger than " + PolyMesh.MINIMUM_COUNT);

        Bpm = bpm;
        SliceCount = sliceCount;
        AccMod = accMod;
        SpeedMod = speedMod;
        SongName = songName;

        //Default values
        PlayedNoteCount = 0;
        Notes = null;
    }

    //Load a dummy map by default
    static Beatmap()
    {
        Load(new Beatmap(bpm: 120, sliceCount: PolyMesh.Instance.Count, accMod: 8, speedMod: 5, songName: "120 bpm drum")
        {
            Notes = new List<Note>
            {
                new Note(time: new Time(ms: 2062), slice: 3),
                new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(1, 120, 4), slice: 3),
                new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(2, 120, 4), slice: 3),
                new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(3, 120, 4), slice: 3),
            }
        });
    }
}
