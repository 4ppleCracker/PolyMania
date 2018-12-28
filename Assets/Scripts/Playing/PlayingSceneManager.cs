using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayingSceneManager : SingletonBehaviour<PlayingSceneManager> {

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

    private void Start()
    {
        AccuracyText = GameObject.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
        ComboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateAccuracyText(100);
        UpdateComboText(0);
        UpdateScoreText(0);
        if (Beatmap.CurrentlyLoaded != null)
        {
            Beatmap.CurrentlyLoaded.Song = Beatmap.GetAudio(Beatmap.CurrentlyLoaded.SongPath);
            Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage);
        } else
        {
            Debug.Log("No beatmap is loaded");
        }
        PolyMesh.Instance.Generate(PolyMesh.Instance.Radius, Beatmap.CurrentlyLoaded?.SliceCount ?? PolyMesh.Instance.Count);
    }

    public Result result = null;

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

    /// <param name="loadedBackground">If you already have the background texture loaded, pass it here</param>
    /// <param name="loadedSong">If you already have the song loaded, pass it here</param>
    public static void StartPlaying(Beatmap map, Texture2D loadedBackground = null, AudioClip loadedSong = null)
    {
        Beatmap.Load(map, map.BackgroundImage ?? loadedBackground, map.Song ?? loadedSong);
        Initiate.Fade("PlayingScene", Color.black, 2);
    }

    public static void GotoResult()
    {
        DontDestroyOnLoad(Instance);
        SceneManager.sceneLoaded += LoadResultsScene;
        Initiate.Fade("ResultsScene", Color.black, 1.5f);
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

    public void Restart()
    {
        StartPlaying(Beatmap.CurrentlyLoaded);
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
    }
}
