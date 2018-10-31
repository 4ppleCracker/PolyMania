using System.Collections.Generic;

public class Beatmap
{
    //Data
    public List<Note> Notes;
    public int PlayedNoteCount = 0;

    //Modifiers
    public readonly int SliceCount;
    public readonly float AccMod;
    public readonly float SpeedMod;
    public int Bpm;

    //Accessors
    public bool AnyNotesLeft => Notes.Count > 0;

    //Loading
    public static Beatmap CurrentlyLoaded { get; private set; }
    public static void Load(Beatmap map)
    {
        CurrentlyLoaded = map;
        PolyMesh.Instance.Generate(3.5f, map.SliceCount);
    }

    //Required so we can check SliceCount
    public Beatmap(int sliceCount, float accMod, float speedMod)
    {
        if (sliceCount < PolyMesh.MINIMUM_COUNT)
            throw new System.Exception("sliceCount parameter must be bigger than " + PolyMesh.MINIMUM_COUNT);

        SliceCount = sliceCount;
        AccMod = accMod;
        SpeedMod = speedMod;
    }

    //Load a dummy map by default
    static Beatmap()
    {
        Load(new Beatmap(sliceCount: 8, accMod: 8, speedMod: 5)
        {
            Notes = new List<Note>
            {
                new Note(time: new Time(ms: 2062), slice: 3),
                new Note(time: 2062 + Conductor.BeatsToTime(1, 120, 4), slice: 3),
                new Note(time: 2062 + Conductor.BeatsToTime(2, 120, 4), slice: 3),
                new Note(time: 2062 + Conductor.BeatsToTime(3, 120, 4), slice: 3),
            }
        });
    }
}
