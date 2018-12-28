using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreListItem : MonoBehaviour
{
    TextMeshProUGUI TotalScore;
    TextMeshProUGUI MaxCombo;
    TextMeshProUGUI TotalAccuracy;

    public Result info = null;

    public void SetScore(Result info)
    {
        TotalScore.text = info.score.ToString("#,##0");
        MaxCombo.text = info.highestCombo + "x";
        TotalAccuracy.text = info.totalAccuracy + "%";

        this.info = info;
    }

    public void Load()
    {
        TotalScore = transform.Find("TotalScore").GetComponent<TextMeshProUGUI>();
        MaxCombo = transform.Find("MaxCombo").GetComponent<TextMeshProUGUI>();
        TotalAccuracy = transform.Find("TotalAccuracy").GetComponent<TextMeshProUGUI>();
    }
}
