using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultSceneManager : SingletonBehaviour<ResultSceneManager> {

    Result result;

    TextMeshProUGUI accuracyListText;
    TextMeshProUGUI accuracyText;

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
        this.result = result;

        accuracyListText = GameObject.Find("AccuracyListText").GetComponent<TextMeshProUGUI>();
        accuracyText = GameObject.Find("AccuracyText").GetComponent<TextMeshProUGUI>();

        string format = accuracyListText.text;
        SetAccuracyList(format);

        accuracyText.text = result.totalAccuracy + "%";
    }
}
