using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour {

    public int noteIndex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Get note from beatmap's notes list
        Note note = Beatmap.CurrentlyLoaded.Notes[noteIndex];

        if (note.clicked)
            Destroy(gameObject);

        //Calculate progress to position where user should click
        float progress = 1 - note.TimeToClick.Ms / NotesController.AllowedTimeToClick;

        //Apply speed mod
        //progress = progress / Beatmap.CurrentlyLoaded.SpeedMod + (1 / Beatmap.CurrentlyLoaded.SpeedMod);

        transform.position = new Vector3(progress * PolyMesh.Instance.Radius, 0);

        //Calculate rotation for slice
        float degrees = 360 * (float)note.slice / Beatmap.CurrentlyLoaded.SliceCount;

        transform.rotation = Quaternion.Euler(0, 0, degrees);
        transform.position = transform.rotation * -transform.position;

        //If note's TimeToClick is later than TimeToMiss, kill the note and set missed to true
        if (-note.TimeToClick.Ms >= NotesController.TimeToMiss)
        {
            Destroy(gameObject);
            note.missed = true;
            Beatmap.CurrentlyLoaded.Notes[noteIndex] = note;
        }
	}
}
