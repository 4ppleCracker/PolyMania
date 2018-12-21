using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NotesController : SingletonBehaviour<NotesController> {

    public static float AllowedTimeToClick => 3000 / Beatmap.CurrentlyLoaded.AccMod;
    public static float ShowTime = 4000 / Beatmap.CurrentlyLoaded.SpeedMod;
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
        if (Beatmap.CurrentlyLoaded.PlayedNotes.Count() == 0)
            return 100;
        int totalAcc = 0;
        int total = Beatmap.CurrentlyLoaded.PlayedNotes.Count();
        for (int i = 0; i < total; i++)
        {
            totalAcc += Beatmap.CurrentlyLoaded.Notes[i].Accuracy.ToPercent();
        }
        return totalAcc / total;
    }

    public Vector3 NoteSize;

    // Use this for initialization
    public void Start ()
    {
        Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage);
        Conductor.Instance.Play(Beatmap.CurrentlyLoaded.Song, 1000, 1000);

        NotePrefab = Resources.Load<GameObject>("Objects/Note");
        NoteSize = NotePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.max;
    }

    public void Click()
    {
        for (int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
        {
            //Positive = early, negative = late
            if ((Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms >= AllowedTimeToClick))
                continue;
            if ((Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms <= -TimeToMiss))
                continue;

            Note note = Beatmap.CurrentlyLoaded.Notes[i];

            if (note.slice == AimController.Instance.SelectedSlice)
            {
                //Calculate accuracy for note
                int accuracy = (int)(note.TimeToClick.Ms * (Beatmap.CurrentlyLoaded.AccMod / 15));

                note.clicked = true;
                note.trueAccuracy = accuracy;

                // Update the note in the list
                Beatmap.CurrentlyLoaded.Notes[Beatmap.CurrentlyLoaded.GetIndexForNote(note)] = note;

                Combo++;
                Score += GetScoreForNote(Combo, note.Accuracy);

                PlayingSceneManager.Instance.UpdateAccuracyText(CurrentAccuracy());

                //To make sure we dont catch 2 notes in 1 tap
                break;
            }
        }
    }

    public GameObject NotePrefab;

    bool noUpdate = false;

    // Update is called once per frame
    void Update ()
    {
        if (noUpdate) return;
        if (Beatmap.CurrentlyLoaded == null) return;
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft)
        {
            if (Conductor.Instance.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Click();
                }
                for (int i = Beatmap.CurrentlyLoaded.PlayedNotes.Count(); i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
                {
                    if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms <= -ShowTime)
                        continue;
                    if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms >= ShowTime)
                        break;

                    Note note = Beatmap.CurrentlyLoaded.Notes[i];
                    if (!note.clicked && !note.generated)
                    {
                        NoteObject noteObject = Instantiate(NotePrefab).GetComponent<NoteObject>();
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
            StartCoroutine(Helper.FadeOut(Conductor.Instance.Player, 1));
            InitResults();
            PlayingSceneManager.GotoResult();

            noUpdate = true;
        }
    }

    public void InitResults()
    {
        Result result = PlayingSceneManager.Instance.result;
        result.resultNotes = new ResultNote[Beatmap.CurrentlyLoaded.Notes.Length];
        for (int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
        {
            Note note = Beatmap.CurrentlyLoaded.Notes[i];
            result.resultNotes[i] = (ResultNote)note;
        }
        result.totalAccuracy = CurrentAccuracy();
        result.highestCombo = highestCombo;
        result.score = Score;
        PlayingSceneManager.Instance.result = result;
    }
}
