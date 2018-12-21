using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongListItem : MonoBehaviour
{
    TextMeshProUGUI SongName = null;

    public void SetBeatmapInfo(BeatmapStoreInfo info)
    {
        if(SongName == null)
            SongName = transform.Find("SongName").GetComponent<TextMeshProUGUI>();
        SongName.text = $"{info.SongName}\n[{info.DifficultyName}]";
    }

    Image image;

    public void SelectedChange(bool selected)
    {
        float scale = selected ? 1.1f : 1;
        Color color = selected ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0.25f);
        transform.localScale = new Vector3(scale, scale);
        if(image == null)
            image = GetComponent<Image>();
        image.color = color;
    }
}
