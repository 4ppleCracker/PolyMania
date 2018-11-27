using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoteObject : MonoBehaviour {

    [NonSerialized]
    public int noteIndex;

    public Note note;

    public float GetDegrees()
    {
        float degrees = 360 * (float)(Beatmap.CurrentlyLoaded.SliceCount - note.slice) / Beatmap.CurrentlyLoaded.SliceCount;
        float correctionDegrees = (360 * 1f / Beatmap.CurrentlyLoaded.SliceCount) / 2;
        degrees -= correctionDegrees;
        return degrees;
    }

    // Update is called once per frame
    void Update ()
    {
        note = Beatmap.CurrentlyLoaded.Notes[noteIndex];

        if (note.clicked)
            Destroy(gameObject);

        int start = (int)(note.time.Ms - NotesController.AllowedTimeToClick);
        int end = note.time.Ms;
        int timeframe = end - start;

        int pos = Conductor.Instance.Position.Ms - start;

        //Calculate progress to position where user should click
        float progress = (float)pos / timeframe;

        Vector2 target = ((PolyMesh.Instance.mesh.vertices[note.slice] + PolyMesh.Instance.mesh.vertices[(note.slice + 1) % Beatmap.CurrentlyLoaded.SliceCount]) / 2);

        Vector2 screenCoords = progress * target;//new Vector2(0, progress * PolyMesh.Instance.Radius);
        transform.position = screenCoords;

        transform.rotation = Quaternion.Euler(0, 0, GetDegrees());

        //If note's TimeToClick is later than TimeToMiss, kill the note and set missed to true
        if (-note.TimeToClick.Ms >= NotesController.TimeToMiss)
        {
            Destroy(gameObject);

            note.Accuracy = new Accuracy(AccuracyType.Miss, true);

            Beatmap.CurrentlyLoaded.Notes[noteIndex] = note;
            Beatmap.CurrentlyLoaded.PlayedNoteCount++;

            NotesController.Instance.Combo = 0;

            NotesController.Instance.UpdateAccuracyText();
        }
	}
}

[CustomEditor(typeof(NoteObject))]
public class NoteObjectEditor : Editor<NoteObject>
{
    private void OnSceneGUI()
    {
        Handles.Label(target.transform.position + Vector3.up * 1.1f, "Rotation: " + target.GetDegrees());
        Handles.Label(target.transform.position + Vector3.up * 1.2f, "Slice: " + target.note.slice);
        Handles.Label(target.transform.position + Vector3.up * 1.3f, "Time to click: " + target.note.TimeToClick);
        Handles.Label(target.transform.position + Vector3.up * 1.4f, "Time: " + target.note.time);
    }
}