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

    [SerializeField]
    private long m_score;
    public long Score {
        get {
            return m_score;
        }
        set {
            m_score = value;
            PlayingSceneManager.Instance.UpdateScoreText(Score);
        }
    }
    [SerializeField]
    private int m_combo;
    public int Combo {
        get {
            return m_combo;
        }
        set {
            m_combo = value;
            PlayingSceneManager.Instance.UpdateComboText(Combo);
        }
    }
    public int highestCombo;

    int GetScoreForNote(int combo, Accuracy acc)
    {
        return combo * acc.ToPercent();
    }

    public int CurrentAccuracy()
    {
        //Amount of notes hit
        int total = Beatmap.CurrentlyLoaded.PlayedNotes.Count();

        //If you have yet hit any notes, you get 100% accuracy
        if (total == 0)
            return 100;

        //Gets the total accuracy percentages and divides by total notes hit to get the average accuracy
        int totalAcc = 0;
        for (int i = 0; i < total; i++)
        {
            totalAcc += Beatmap.CurrentlyLoaded.Notes[i].Accuracy.ToPercent();
        }
        return totalAcc / total;
    }

    /// <summary>
    /// Prefab of the playing note
    /// </summary>
    public GameObject NotePrefab;

    /// <summary>
    /// Size of the note
    /// </summary>
    public Vector3 NoteSize;

    // Use this for initialization
    public void Start ()
    {
        //if a beatmap is loaded
        if (Beatmap.CurrentlyLoaded != null)
        {
            //set background and start the song with delay and fade time
            Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage);
            Conductor.Instance.Play(Beatmap.CurrentlyLoaded.Song, delay: 1000, fadeTime: 1000);
        }

        //Load note prefab from resources
        NotePrefab = Resources.Load<GameObject>("Objects/Note");

        //load size of the note
        NoteSize = NotePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.max;
    }

    public Accuracy ClickNote(Note note)
    {
        //Calculate accuracy for note
        int accuracy = (int)(note.TimeToClick.Ms * (Beatmap.CurrentlyLoaded.AccMod / 15));

        //Set the data
        note.clicked = true;
        note.trueAccuracy = accuracy;

        //save the note into the beatmap
        Beatmap.CurrentlyLoaded.Notes[Beatmap.CurrentlyLoaded.GetIndexForNote(note)] = note;

        //add combo and score
        Combo++;
        Score += GetScoreForNote(Combo, note.Accuracy);

        //update accuracy text
        PlayingSceneManager.Instance.UpdateAccuracyText(CurrentAccuracy());

        return note.Accuracy;
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
                Accuracy accuracy = ClickNote(note);

                //TODO display hit accuracy

                //To make sure we dont catch 2 notes in 1 tap
                break;
            }
        }
    }

    //used for when we want to kill notes controller but cant destroy it due to fading
    bool noUpdate = false;

    // Update is called once per frame
    void Update ()
    {
        if (noUpdate) return;

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
            //if current combo is higher than higest combo, set highest combo to current combo
            if (highestCombo < Combo)
                highestCombo = Combo;
        }
        else
        {
            //Fade out the music
            StartCoroutine(Helper.FadeOut(Conductor.Instance.Player, 1)); 
            //Load the results which will be sent to result manager
            InitResults();
            //load the result scene
            PlayingSceneManager.GotoResult();

            //make sure that update function isnt called anymore
            noUpdate = true;
        }
    }

    public void InitResults()
    {
        //create result instance
        Result result = new Result();

        //set result the result notes
        result.resultNotes = new ResultNote[Beatmap.CurrentlyLoaded.Notes.Length];
        for (int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
        {
            Note note = Beatmap.CurrentlyLoaded.Notes[i];
            result.resultNotes[i] = (ResultNote)note;
        }

        //set accuracy, combo and score
        result.totalAccuracy = CurrentAccuracy();
        result.highestCombo = highestCombo;
        result.score = Score;
        result.uuid = Beatmap.CurrentlyLoaded.GetUUID();

        //put the result into playing scene manager so it can be pushed to result scene manager by GotoResult
        PlayingSceneManager.Instance.result = result;
    }
}
