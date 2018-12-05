using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NotesController : SingletonBehaviour<NotesController> {

    public static float AllowedTimeToClick => 4000 / Beatmap.CurrentlyLoaded.AccMod;
    public static float TimeToMiss => 2000 / Beatmap.CurrentlyLoaded.AccMod;

    Result result;

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
        if (Beatmap.CurrentlyLoaded.PlayedNoteCount == 0)
            return 100;
        int totalAcc = 0;
        for (int i = 0; i < Beatmap.CurrentlyLoaded.PlayedNoteCount; i++)
        {
            totalAcc += Beatmap.CurrentlyLoaded.Notes[i].Accuracy.ToPercent();
        }
        return totalAcc / Beatmap.CurrentlyLoaded.PlayedNoteCount;
    }    

    // Use this for initialization
    public void Start ()
    {
        Conductor.Instance.Play();
        result = new Result();
        Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage);
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
                            Beatmap.CurrentlyLoaded.Notes[Beatmap.CurrentlyLoaded.GetIndexForNote(note)] = note;
                            Beatmap.CurrentlyLoaded.PlayedNoteCount++;

                            Combo++;
                            Score += GetScoreForNote(Combo, note.Accuracy);

                            PlayingSceneManager.Instance.UpdateAccuracyText(CurrentAccuracy());
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
            if (highestCombo < Combo)
                highestCombo = Combo;
        }
        else
        {
            SceneManager.LoadSceneAsync("ResultsScene", LoadSceneMode.Additive).completed += delegate
            {
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
                    throw new Exception("No scene manager found");
                }

                System.Threading.Thread.Sleep(1000);

                SceneManager.UnloadSceneAsync("PlayingScene").completed += delegate
                {
                    resultSceneManager.Load(result);
                };
            };

            result.resultNotes = new ResultNote[Beatmap.CurrentlyLoaded.Notes.Count];
            for(int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Count; i++)
            {
                Note note = Beatmap.CurrentlyLoaded.Notes[i];
                result.resultNotes[i] = (ResultNote)note;
            }
            result.totalAccuracy = CurrentAccuracy();
            result.highestCombo = highestCombo;
            result.score = Score;

            Destroy(this);
        }
    }
}
