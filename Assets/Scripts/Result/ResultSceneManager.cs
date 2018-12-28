using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultSceneManager : SingletonBehaviour<ResultSceneManager> {

    Result result;

    TextMeshProUGUI accuracyListText;
    TextMeshProUGUI accuracyText;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI highestComboText;

    public void SetAccuracyList(string format)
    {
        AccuracyType[] accArray = (AccuracyType[])Enum.GetValues(typeof(AccuracyType));
        object[] counts = new object[accArray.Length];
        for(int i = 0; i < accArray.Length; i++)
        {
            counts[i] = result.GetCountForAccuracy(accArray[i]);
        }
        accuracyListText.text = string.Format(format, counts);
    }

    public void Load(Result result)
    {
        Debug.Log("Loading results");
        this.result = result;

        Debug.Log("Loading UI");
        accuracyListText = GameObject.Find("AccuracyListText").GetComponent<TextMeshProUGUI>();
        accuracyText = GameObject.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        highestComboText = GameObject.Find("HighestComboText").GetComponent<TextMeshProUGUI>();

        string format = accuracyListText.text;
        SetAccuracyList(format);

        accuracyText.text = "Accuracy: " + result.totalAccuracy + "%";

        scoreText.text = "Score: " + result.score.ToString("#,##0");

        highestComboText.text = "Highest Combo: " + result.highestCombo + "x";

        Debug.Log("Saving score to disk");

        ScoreStore.AddOfflineScore(result);

        Debug.Log("Setting background");

        Helper.SetBackgroundImage(Beatmap.CurrentlyLoaded.BackgroundImage, 0.25f);

        Debug.Log("Done");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Initiate.Fade("SongSelectScene", Color.black, 3);
        }
    }
}
