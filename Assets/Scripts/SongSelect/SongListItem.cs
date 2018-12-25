using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongListItem : MonoBehaviour
{
    TextMeshProUGUI SongNameText = null;
    public BeatmapStoreInfo info = null;

    public BeatmapStoreInfo GetInfo()
    {
        return info;
    }
    public void SetBeatmapInfo(BeatmapStoreInfo info)
    {
        if(SongNameText == null)
            SongNameText = transform.Find("SongName").GetComponent<TextMeshProUGUI>();
        SongNameText.text = $"{info.SongName}\n[{info.DifficultyName}]";

        this.info = info;
    }

    Image image;

    public void SelectedChange(bool selected)
    {
        float scale = selected ? 1.1f : 1;
        transform.localScale = new Vector3(scale, scale);

        Color color = selected ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0.25f);
        (image ?? (image = GetComponent<Image>())).color = color;
    }
}
