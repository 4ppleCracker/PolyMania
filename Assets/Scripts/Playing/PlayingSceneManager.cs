using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayingSceneManager : SingletonBehaviour<PlayingSceneManager> {

    [SerializeField]
    private long m_score;
    public long Score {
        get {
            return m_score;
        }
        set {
            m_score = value;
            UpdateScoreText(Score);
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
            UpdateComboText(Combo);
        }
    }
    public int highestCombo;

    TextMeshProUGUI AccuracyText;
    TextMeshProUGUI ScoreText;
    TextMeshProUGUI ComboText;

    public void UpdateAccuracyText(int accuracy)
    {
        AccuracyText.text = accuracy + "%";
    }
    public void UpdateComboText(int combo)
    {
        ComboText.text = combo + "x";
    }
    public void UpdateScoreText(long score)
    {
        ScoreText.text = score.ToString("#,##0");
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

    private void Start()
    {
        AccuracyText = GameObject.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
        ComboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        m_score = 0;
        m_combo = 0;
        highestCombo = 0;
        UpdateAccuracyText(100);
        UpdateComboText(0);
        UpdateScoreText(0);
        if (Beatmap.CurrentlyLoaded != null)
        {
            //set background and start the song with delay and fade time
            Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage);
            Beatmap.CurrentlyLoaded.Song = Beatmap.GetAudio(Beatmap.CurrentlyLoaded.SongPath);
            Conductor.Instance.Play(Beatmap.CurrentlyLoaded.Song, delay: 1000, fadeTime: 1000);
        } else
        {
            Debug.Log("No beatmap is loaded");
        }
        PolyMesh.Instance.Generate(PolyMesh.Instance.Radius, Beatmap.CurrentlyLoaded?.SliceCount ?? PolyMesh.Instance.Count);
    }

    public Result result = null;

    /// <param name="loadedBackground">If you already have the background texture loaded, pass it here</param>
    /// <param name="loadedSong">If you already have the song loaded, pass it here</param>
    public static void StartPlaying(Beatmap map, Texture2D loadedBackground = null, AudioClip loadedSong = null)
    {
        Beatmap.Load(map, map.BackgroundImage ?? loadedBackground, map.Song ?? loadedSong);
        Initiate.Fade("PlayingScene", Color.black, 2.5f);
    }

    public static void GotoResult()
    {
        DontDestroyOnLoad(Instance);
        SceneManager.sceneLoaded += LoadResultsScene;
        Initiate.Fade("ResultsScene", Color.black, 2f);
    }
    private static void LoadResultsScene(Scene resultScene, LoadSceneMode y)
    {
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

        resultSceneManager.Load(Instance.result);

        Destroy(Instance);
        SceneManager.sceneLoaded -= LoadResultsScene;
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
        Score += NotesController.GetScoreForNote(Combo, note.Accuracy);

        return note.Accuracy;
    }

    int holdTime = 250;
    IEnumerator CheckRHold()
    {
        DateTime end = DateTime.Now.AddMilliseconds(holdTime);
        while (DateTime.Now < end)
        {
            if (!Input.GetKey(KeyCode.R))
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Restarting..");
        Restart();
    }
    public void Restart()
    {
        NotesController.Instance.Reset();
        Beatmap.CurrentlyLoaded.Reload();
        Start();
    }

    bool paused;
    public void Pause()
    {
        paused = !paused;
        if(paused)
        {
            Conductor.Instance.Player.Pause();
        }
        else
        {
            Conductor.Instance.Player.UnPause();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(CheckRHold());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
        //if current combo is higher than higest combo, set highest combo to current combo
        if (highestCombo < Combo)
            highestCombo = Combo;
        //Update accuracy
        UpdateAccuracyText(CurrentAccuracy());
    }

    public void InitResults()
    {
        //create result instance
        result = new Result
        {
            totalAccuracy = CurrentAccuracy(),
            highestCombo = highestCombo,
            score = Score,
            uuid = Beatmap.CurrentlyLoaded.GetUUID(),
            date = DateTime.Now,

            resultNotes = new ResultNote[Beatmap.CurrentlyLoaded.Notes.Length]
        };
        for (int i = 0; i < Beatmap.CurrentlyLoaded.Notes.Length; i++)
        {
            Note note = Beatmap.CurrentlyLoaded.Notes[i];
            result.resultNotes[i] = (ResultNote)note;
        }
    }
}
