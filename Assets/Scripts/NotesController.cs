using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesController : SingletonBehaviour<NotesController> {

    public static float AllowedTimeToClick => 4000 / Beatmap.CurrentlyLoaded.AccMod;
    public static float TimeToMiss => 0;//4000 / Beatmap.CurrentlyLoaded.AccMod;

    // Use this for initialization
    void Start () {
        Conductor.Instance.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft && Input.GetKeyDown(KeyCode.Space))
        {
            Note note = Beatmap.CurrentlyLoaded.Notes[Beatmap.CurrentlyLoaded.PlayedNoteCount];
            if (Conductor.Instance.Position >= note.time - AllowedTimeToClick)
            {
                //Calculate accuracy for note
                int accuracy = (int)(note.TimeToClick.Ms * Beatmap.CurrentlyLoaded.AccMod);

                note.clicked = true;
                note.accuracy = accuracy;

                // Update the note in the list
                Beatmap.CurrentlyLoaded.Notes[Beatmap.CurrentlyLoaded.PlayedNoteCount] = note;

                Debug.Log(accuracy);

                Beatmap.CurrentlyLoaded.PlayedNoteCount++;
            }
        }
        for(int i = Beatmap.CurrentlyLoaded.PlayedNoteCount; i < Beatmap.CurrentlyLoaded.Notes.Count; i++)
        {
            if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms <= -AllowedTimeToClick)
                continue;
            if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms >= AllowedTimeToClick)
                break;
            Note note = Beatmap.CurrentlyLoaded.Notes[i];
            if (!note.clicked && !note.generated)
            {
                NoteObject noteObject = Instantiate(Resources.Load<GameObject>("Objects/Note")).GetComponent<NoteObject>();
                noteObject.noteIndex = i;
                note.generated = true;
                Beatmap.CurrentlyLoaded.Notes[i] = note;
            }
        }
    }
}
