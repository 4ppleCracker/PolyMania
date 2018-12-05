using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;

[CreateAssetMenu]
public class Beatmap : ScriptableObject

    ////Loaded data
{
    //Metadata
    public string SongName;

    //Data
    public List<Note> Notes = new List<Note>();
    public Texture2D BackgroundImage = null;

    //Modifiers
    public readonly int SliceCount;
    public readonly float AccMod;
    public readonly float SpeedMod;
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

    //Load a dummy map by default
    static Beatmap()
    {
        Load(
            new Beatmap(
                bpm: 120,
                sliceCount: 6,
                accMod: 8, speedMod: 5,
                songName: "120 bpm drum",
                background: Resources.Load<Texture2D>("Textures/testBackground")
            )
            {
                Notes = new List<Note>
                {
                    new Note(time: new Time(ms: 2062)                                   , slice: 0),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(1, 120, 4), slice: 3),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(2, 120, 4), slice: 1),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(3, 120, 4), slice: 4),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(4, 120, 4), slice: 2),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(5, 120, 4), slice: 5),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(6, 120, 4), slice: 3),
                    new Note(time: new Time(ms: 2062) + Conductor.BeatsToTime(7, 120, 4), slice: 0),
                }
            }
        );
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Beatmap))]
public class BeatmapEditor : Editor<Beatmap>
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        target.SongName = EditorGUILayout.TextField("Song Name", target.SongName);
        target.Bpm = EditorGUILayout.IntField("Bpm", target.Bpm);
        target.BackgroundImage = (Texture2D)EditorGUILayout.ObjectField("Image", target.BackgroundImage, typeof(Texture2D), false);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("Notes"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif