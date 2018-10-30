using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesController : SingletonBehaviour<NotesController> {

	// Use this for initialization
	void Start () {
        Conductor.Instance.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft)
        {
            Note note;
            if (Conductor.Instance.Position >= (note = Beatmap.CurrentlyLoaded.Notes.Peek()).time)
            {
                Debug.Log(note.slice);
            }
        }
    }
}
