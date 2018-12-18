using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NotesController : SingletonBehaviour<NotesController> {

    public static float AllowedTimeToClick => 4000 / Beatmap.CurrentlyLoaded.SpeedMod;
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
        Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage);
        result = new Result();
        Conductor.Instance.Play(Beatmap.CurrentlyLoaded.Song);
    }

    bool noUpdate = false;

    // Update is called once per frame
    void Update ()
    {
        if (noUpdate) return;
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft)
        {
            if (Conductor.Instance.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    for (int i = Beatmap.CurrentlyLoaded.PlayedNoteCount; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
                    {
                        if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms <= -AllowedTimeToClick)
                            continue;
                        if (Beatmap.CurrentlyLoaded.Notes[i].TimeToClick.Ms >= AllowedTimeToClick)
                            break;

                        Note note = Beatmap.CurrentlyLoaded.Notes[i];

                        if (Conductor.Instance.Position >= note.time - (int)AllowedTimeToClick && note.slice == AimController.Instance.SelectedSlice)
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

                            //To make sure we dont catch 2 notes in 1 tap
                            break;
                        }
                    }
                }
                for (int i = Beatmap.CurrentlyLoaded.PlayedNoteCount; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
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
            StartCoroutine(Helper.FadeOut(Conductor.Instance.Player, 1));
            SceneManager.sceneLoaded += UnloadPlayingScene;
            Initiate.Fade("ResultsScene", Color.black, 1);
            SceneManager.sceneLoaded -= UnloadPlayingScene;

            result.resultNotes = new ResultNote[Beatmap.CurrentlyLoaded.Notes.Length];
            for(int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
            {
                Note note = Beatmap.CurrentlyLoaded.Notes[i];
                result.resultNotes[i] = (ResultNote)note;
            }
            result.totalAccuracy = CurrentAccuracy();
            result.highestCombo = highestCombo;
            result.score = Score;

            noUpdate = true;
        }
    }

    private void UnloadPlayingScene(Scene arg0, LoadSceneMode arg1)
    {
        Scene resultScene = SceneManager.GetSceneByName("ResultsScene");

        ResultSceneManager resultSceneManager = null;

        foreach (GameObject obj in resultScene.GetRootGameObjects())
        {
            ResultSceneManager comp;
            if ((comp = obj.GetComponent<ResultSceneManager>()) != null)
            {
                resultSceneManager = comp;
            }
        }

        if (resultSceneManager == null)
        {
            throw new Exception("No scene manager found");
        }

        SceneManager.UnloadSceneAsync("PlayingScene").completed += delegate
        {
            resultSceneManager.Load(result);
        };
    }
}
