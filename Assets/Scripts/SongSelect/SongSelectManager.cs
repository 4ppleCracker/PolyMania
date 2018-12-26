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

    private Texture2D background = null;

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

            BeatmapStoreInfo oldInfo = null;

            //So you dont deselect out of bounds items
            if (m_selected >= 0 && m_selected < SongListContent.transform.childCount)
            {
                SongListItem old = SongListContent.transform.GetChild(m_selected).GetComponent<SongListItem>();

                //deselect old selected item
                old.SelectedChange(false);
                //get the old info for optimization
                oldInfo = old.info;
            }

            //Set new index
            m_selected = value;

            //get the the songlistitem component from the child with new index
            SongListItem item = SongListContentTransform.GetChild(value).GetComponent<SongListItem>();

            //call select change on the item
            item.SelectedChange(true);

            //get the beatmap info of the item
            BeatmapStoreInfo info = item.info;

            //set background image if its not the same as the old one
            if (oldInfo?.BackgroundPath != info.BackgroundPath)
            {
                background = Helper.LoadPNG(info.BackgroundPath);
                Helper.SetBackgroundImage(background, 0.75f);
            }
        }
    }

    public float itemHeight = -1;

    public float CoordinateForIndex(int index)
    {
        return -YOffset - ((YSpacing + itemHeight) * index);
    }

    public void AddSongItem(int index, BeatmapStoreInfo info)
    {
        GameObject songListItem = Instantiate(SongListItemPrefab, SongListContentTransform);
        RectTransform rect = songListItem.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0, CoordinateForIndex(index));
        SongListItem item = songListItem.GetComponent<SongListItem>();
        item.Load();
        item.SetBeatmapInfo(info);
        item.SelectedChange(false);
    }

    RectTransform SongListContentTransform;

    public void Start()
    {
        SongListContentTransform = SongListContent.GetComponent<RectTransform>();

        if (BeatmapStore.Beatmaps == null)
            BeatmapStore.LoadAll();

        itemHeight = SongListItemPrefab.GetComponent<RectTransform>().rect.height;

        int i = 0;
        foreach(BeatmapStoreInfo info in BeatmapStore.Beatmaps)
        {
            AddSongItem(i, info);

            i++;
        }

        //Resize the content transform to match the size of the list
        RectTransform last = SongListContentTransform.GetChild(SongListContentTransform.childCount - 1).GetComponent<RectTransform>();
        RectTransform rect = SongListContent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, -last.localPosition.y + last.sizeDelta.y / 2 + YOffset / 2);

        Selected = UnityEngine.Random.Range(0, BeatmapStore.Beatmaps.Count);
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
            Beatmap beatmap = BeatmapStore.Beatmaps[Selected].GetBeatmap();
            PlayingSceneManager.StartPlaying(beatmap, background);
        }
    }
}
