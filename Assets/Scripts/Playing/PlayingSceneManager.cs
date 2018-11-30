using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        Debug.Log("combo");
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
}
