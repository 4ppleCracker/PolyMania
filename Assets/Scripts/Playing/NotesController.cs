using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NotesController : SingletonBehaviour<NotesController> {

    public static float AllowedTimeToClick => 3000 / Beatmap.CurrentlyLoaded.AccMod;
    public static float ShowTime => 4000 / Beatmap.CurrentlyLoaded.SpeedMod;
    public static float TimeToMiss => 3000 / Beatmap.CurrentlyLoaded.AccMod;

    public static int GetScoreForNote(int combo, Accuracy acc)
    {
        return combo * acc.ToPercent();
    }  

    /// <summary>
    /// Prefab of the playing note
    /// </summary>
    public GameObject NotePrefab;

    /// <summary>
    /// Size of the note
    /// </summary>
    public Vector3 NoteSize;

    public void Reset()
    {
        foreach (NoteObject obj in FindObjectsOfType<NoteObject>())
            Destroy(obj.gameObject);
    }

    // Use this for initialization
    public void Start ()
    {
        //Load note prefab from resources
        NotePrefab = Resources.Load<GameObject>("Objects/Note");

        //load size of the note
        NoteSize = NotePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.max;
    }

    public void CheckClick()
    {
        //go through each note of the beatmap
        for (int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
        {
            //load current iteration's note from the beatmap into a variable
            Note note = Beatmap.CurrentlyLoaded.Notes[i];

            //if the correct slice is aimed at, song position is within the hit frame of the note, and the note isnt clicked
            if (note.slice == AimController.Instance.SelectedSlice && Conductor.Instance.Position.IsWithin(note.HitTimeFrame) && !note.clicked)
            {
                //click the note
                Accuracy accuracy = PlayingSceneManager.Instance.ClickNote(note);

                //TODO display hit accuracy

                //To make sure we dont catch 2 notes in 1 tap
                break;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //dont do logic if there is no song loaded
        if (Beatmap.CurrentlyLoaded == null) return;
        //dont do logic if there are no notes left
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft)
        {
            //dont do logic if the song isnt playing
            if (Conductor.Instance.Playing)
            {
                //If space key is pressed, call the click code
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CheckClick();
                }
                //Go through each note in the loaded beatmap
                for (int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
                {
                    //load the current iteration's note from the beatmap into a variable
                    Note note = Beatmap.CurrentlyLoaded.Notes[i];

                    //substracts time to show with the spawn time
                    //for example:
                    //  if its supposed to spawn at 2000 ms and showtime is a 500 ms.
                    //  it should spawn at 1500 ms, which is what timeToShowNote represent
                    Time timeToShowNote = note.time.SubtractMs((int)ShowTime);
                    //If song position is not at the time to show note, skip this iteration
                    if (Conductor.Instance.Position <= timeToShowNote)
                        continue;

                    //Dont spawn the note if its already been clicked/generated
                    if (!note.clicked && !note.generated)
                    {
                        //instantiate a new note using the prefab and get the noteobject component
                        NoteObject noteObject = Instantiate(NotePrefab).GetComponent<NoteObject>();
                        //set the correct note index for use in the note object script
                        noteObject.noteIndex = i;
                        //set generated to true so we dont spawn it twice
                        note.generated = true;
                        //save the note back into the beatmap
                        Beatmap.CurrentlyLoaded.Notes[i] = note;
                    }
                }
            }
        }
        else
        {
            //Fade out the music
            StartCoroutine(Helper.FadeOut(Conductor.Instance.Player, 1)); 
            //Load the results which will be sent to result manager
            PlayingSceneManager.Instance.InitResults();
            //load the result scene
            PlayingSceneManager.GotoResult();

            Destroy(gameObject);
        }
    }
}
