using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NotesController : SingletonBehaviour<NotesController> {

    public static float AllowedTimeToClick => 4000 / Beatmap.CurrentlyLoaded.AccMod;
    public static float TimeToMiss => 2000 / Beatmap.CurrentlyLoaded.AccMod;

    Result result;

    // Use this for initialization
    void Start ()
    {
        Conductor.Instance.Play();
        result = new Result();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft)
        {
            if (Conductor.Instance.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    for (int i = Beatmap.CurrentlyLoaded.PlayedNoteCount; i < Beatmap.CurrentlyLoaded.Notes.Count; i++)
                    {
                        if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms <= -AllowedTimeToClick)
                            continue;
                        if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms >= AllowedTimeToClick)
                            break;

                        Note note = Beatmap.CurrentlyLoaded.Notes[i];

                        if (Conductor.Instance.Position >= note.time - AllowedTimeToClick && note.slice == AimController.Instance.SelectedSlice)
                        {
                            //Calculate accuracy for note
                            int accuracy = (int)(note.TimeToClick.Ms * (Beatmap.CurrentlyLoaded.AccMod / 15));

                            note.clicked = true;
                            note.trueAccuracy = accuracy;

                            // Update the note in the list
                            Beatmap.CurrentlyLoaded.Notes[Beatmap.CurrentlyLoaded.PlayedNoteCount] = note;

                            Debug.Log(note.Accuracy + " " + accuracy + " ms");

                            Beatmap.CurrentlyLoaded.PlayedNoteCount++;
                        }
                    }
                }
                for (int i = Beatmap.CurrentlyLoaded.PlayedNoteCount; i < Beatmap.CurrentlyLoaded.Notes.Count; i++)
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
        else
        {
            SceneManager.LoadSceneAsync("ResultsScene", LoadSceneMode.Additive).completed += delegate
            {
                SceneManager.UnloadSceneAsync("PlayingScene");

                Scene resultScene = SceneManager.GetSceneByName("ResultsScene");

                ResultSceneManager resultSceneManager = null;

                foreach(GameObject obj in resultScene.GetRootGameObjects())
                {
                    ResultSceneManager comp;
                    if ((comp = obj.GetComponent<ResultSceneManager>()) != null)
                    {
                        resultSceneManager = comp;
                    }
                }

                if(resultSceneManager == null)
                {
                    throw new System.Exception("No scene manager found");
                }

                resultSceneManager.Load(result);
            };

            result.resultNotes = new ResultNote[Beatmap.CurrentlyLoaded.Notes.Count];
            for(int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Count; i++)
            {
                Note note = Beatmap.CurrentlyLoaded.Notes[i];
                result.resultNotes[i] = (ResultNote)note;
            }

            Destroy(this);
        }
    }
}
