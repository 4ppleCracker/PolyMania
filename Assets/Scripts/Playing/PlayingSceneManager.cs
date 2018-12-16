using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    // Use this for initialization
    void Start () {
        AccuracyText = GameObject.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
        ComboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        UpdateAccuracyText(100);
        UpdateComboText(0);
        UpdateScoreText(0);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Beatmap.CurrentlyLoaded.Reload();
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
