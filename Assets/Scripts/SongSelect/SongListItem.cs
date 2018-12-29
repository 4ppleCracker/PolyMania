using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongListItem : MonoBehaviour
{
    TextMeshProUGUI SongNameText = null;
    public BeatmapStoreInfo info = null;

    public void SetBeatmapInfo(BeatmapStoreInfo info)
    {
        if(SongNameText == null)
            SongNameText = transform.Find("SongName").GetComponent<TextMeshProUGUI>();
        SongNameText.text = $"{info.RomanizedSongName}\n[{info.DifficultyName}]";

        this.info = info;
    }

    public void Load()
    {
        image = GetComponent<Image>();
    }

    Image image;

    static Vector3 SelectedScale = new Vector3(1.1f, 1.1f);
    static Vector3 UnselectedScale = new Vector3(1f, 1f);

    static Color SelectedColor = new Color(1, 1, 1, 1);
    static Color UnselectedColor = new Color(0, 0, 0, 0.25f);

    public void SelectedChange(bool selected)
    {
        transform.localScale = selected ? SelectedScale : UnselectedScale;
        image.color = selected ? SelectedColor : UnselectedColor;
    }
}
