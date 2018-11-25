using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultSceneManager : SingletonBehaviour<ResultSceneManager> {

    Result result;

    TextMeshProUGUI accuracyText;

    public void SetAccuracies(string format)
    {
        AccuracyType[] accArray = (AccuracyType[])Enum.GetValues(typeof(AccuracyType));
        object[] counts = new object[accArray.Length];
        for(int i = 0; i < accArray.Length; i++)
        {
            counts[i] = result.GetCountForAccuracy(accArray[i]);
        }
        accuracyText.text = string.Format(format, counts);
    }

    public void Load(Result result)
    {
        this.result = result;
        accuracyText = GameObject.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
        string format = accuracyText.text;
        SetAccuracies(format);
    }
}
