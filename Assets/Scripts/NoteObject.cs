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
        float degrees = 360 * (float)(note.slice + 1) / Beatmap.CurrentlyLoaded.SliceCount;
        float correctionDegrees = (360 * 1f / Beatmap.CurrentlyLoaded.SliceCount) / 2;
        float extraDegrees = 90;
        degrees -= correctionDegrees;
        degrees -= extraDegrees;
        return degrees;
    }

    // Update is called once per frame
    void Update ()
    {
        note = Beatmap.CurrentlyLoaded.Notes[noteIndex];

        if (note.clicked)
            Destroy(gameObject);

        //Calculate progress to position where user should click
        float progress = 1 - note.TimeToClick.Ms / NotesController.AllowedTimeToClick;

        //Apply speed mod
        //progress = progress / Beatmap.CurrentlyLoaded.SpeedMod + (1 / Beatmap.CurrentlyLoaded.SpeedMod);

        transform.position = new Vector3(0, progress * PolyMesh.Instance.Radius);

        transform.rotation = Quaternion.Euler(0, 0, GetDegrees());
        transform.position = transform.rotation * -transform.position;

        //If note's TimeToClick is later than TimeToMiss, kill the note and set missed to true
        if (-note.TimeToClick.Ms >= NotesController.TimeToMiss)
        {
            Destroy(gameObject);
            note.missed = true;
            Beatmap.CurrentlyLoaded.Notes[noteIndex] = note;
            Beatmap.CurrentlyLoaded.PlayedNoteCount++;
            EditorApplication.Beep();
            Debug.Log("Missed " + noteIndex);
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
    }
}