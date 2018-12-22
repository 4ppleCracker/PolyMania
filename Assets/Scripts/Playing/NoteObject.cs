using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void Start()
    {
        Update();
    }

    // Update is called once per frame
    void Update ()
    {
        note = Beatmap.CurrentlyLoaded.Notes[noteIndex];

        if (note.clicked)
            Destroy(gameObject);

        int start = (int)(note.time.Ms - NotesController.ShowTime);
        int end = note.time.Ms;
        int timeframe = end - start;

        int pos = Conductor.Instance.Position.Ms - start;

        //Calculate progress to position where user should click
        float progress = (float)pos / timeframe;

        Vector2 target = ((PolyMesh.Instance.mesh.vertices[note.slice] + PolyMesh.Instance.mesh.vertices[(note.slice + 1) % Beatmap.CurrentlyLoaded.SliceCount]) / 2 - new Vector3(0, (NotesController.Instance.NoteSize.y / 2)));

        Vector2 screenCoords = progress * target;//new Vector2(0, progress * PolyMesh.Instance.Radius);
        transform.position = screenCoords;

        transform.rotation = Quaternion.Euler(0, 0, GetDegrees());

        //If note's TimeToClick is later than TimeToMiss, kill the note and set missed to true
        if (Conductor.Instance.Position.IsAfter(note.HitTimeFrame))
        {
            note.Accuracy = new Accuracy(AccuracyType.Miss, true);
            note.clicked = true;

            Beatmap.CurrentlyLoaded.Notes[noteIndex] = note;

            NotesController.Instance.Combo = 0;

            PlayingSceneManager.Instance.UpdateAccuracyText(NotesController.Instance.CurrentAccuracy());

            Destroy(gameObject);
        }
	}
}
