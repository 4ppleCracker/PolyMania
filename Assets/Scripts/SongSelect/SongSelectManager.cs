using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SongSelectManager : SingletonBehaviour<SongSelectManager>
{
    [SerializeField]
    public GameObject SongListContent;
    [SerializeField]
    public GameObject SongListItemPrefab;

    public int YOffset = 25;
    public int YSpacing = 25;

    [SerializeField]
    private int m_selected = -1;
    public int Selected {
        get {
            return m_selected;
        }
        set {
            //Bounds checking for the new index
            if (value < 0 || value >= SongListContent.transform.childCount)
                return;

            //So you dont deselect out of bounds items
            if (m_selected >= 0 && m_selected < SongListContent.transform.childCount)
                SongListContent.transform.GetChild(m_selected).GetComponent<SongListItem>().SelectedChange(false);

            //Set new index and call select the item
            m_selected = value;
            SongListItem item = SongListContent.transform.GetChild(value).GetComponent<SongListItem>();
            item.SelectedChange(true);
            BeatmapStoreInfo info = item.GetInfo();
            Texture2D background = Helper.LoadPNG(info.BackgroundPath);
            Helper.SetBackgroundImage(background, 0.75f);

        }
    }

    public float itemHeight = -1;

    public float CoordinateForIndex(int index)
    {
        return -YOffset - ((YSpacing + itemHeight) * index);
    }

    public void AddSongItem(int index, BeatmapStoreInfo info)
    {
        GameObject songListItem = Instantiate(SongListItemPrefab, SongListContent.transform);
        RectTransform rect = songListItem.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0, CoordinateForIndex(index));
        SongListItem item = songListItem.GetComponent<SongListItem>();
        item.SetBeatmapInfo(info);
        item.SelectedChange(false);
    }

    public void Start()
    {
        BeatmapStore.LoadAll();
        itemHeight = SongListItemPrefab.GetComponent<RectTransform>().rect.height;

        int i = 0;
        foreach(BeatmapStoreInfo info in BeatmapStore.Beatmaps)
        {
            AddSongItem(i, info);

            i++;
        }

        //Resize the content transform to match the size of the list
        RectTransform last = SongListContent.transform.GetChild(SongListContent.transform.childCount - 1).GetComponent<RectTransform>();
        RectTransform rect = SongListContent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, -last.localPosition.y + last.sizeDelta.y / 2 + YOffset / 2);

        Selected = UnityEngine.Random.Range(0, BeatmapStore.Beatmaps.Count);
        float selectedYCoordinate = CoordinateForIndex(Selected);

    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Selected++;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Selected--;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayingSceneManager.StartPlaying(BeatmapStore.Beatmaps[Selected].GetBeatmap());
        }
    }
}
