using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SongSelectManager : SingletonBehaviour<SongSelectManager>
{
    [SerializeField]
    public RectTransform SongListContentTransform;
    [SerializeField]
    public GameObject SongListItemPrefab;

    [SerializeField]
    public RectTransform ScoreListContentTransform;
    [SerializeField]
    public GameObject ScoreListItemPrefab;

    Rect SongListItemRect;
    Rect ScoreListItemRect;

    public int SongListYOffset = 25;
    public int SongListYSpacing = 25;

    public int ScoreListYOffset = 25;
    public int ScoreListYSpacing = 25;

    private Texture2D background = null;

    private static int m_selected = -1;
    public static int Selected {
        get {
            return m_selected;
        }
        set {
            if (Instance == null)
            {
                m_selected = value;
                return;
            }
            //Bounds checking for the new index
            if (value < 0 || value >= Instance.SongListContentTransform.childCount)
                return;

            BeatmapStoreInfo oldInfo = null;

            //So you dont deselect out of bounds items
            if (m_selected >= 0 && m_selected < Instance.SongListContentTransform.childCount)
            {
                SongListItem old = Instance.SongListContentTransform.GetChild(m_selected).GetComponent<SongListItem>();

                //deselect old selected item
                old.SelectedChange(false);
                //get the old info for optimization
                oldInfo = old.info;

                //remove old score listing
                foreach (Transform child in Instance.ScoreListContentTransform)
                {
                    Destroy(child.gameObject);
                }
            }

            //Set new index
            m_selected = value;

            //get the the songlistitem component from the child with new index
            SongListItem item = Instance.SongListContentTransform.GetChild(value).GetComponent<SongListItem>();

            //call select change on the item
            item.SelectedChange(true);

            //get the beatmap info of the item
            BeatmapStoreInfo info = item.info;

            //set background image if its not the same as the old one
            if (oldInfo?.BackgroundPath != info.BackgroundPath || Instance.isStart)
            {
                Instance.background = Helper.LoadPNG(info.BackgroundPath);
                Helper.SetBackgroundImage(Instance.background, 0.75f);
            }

            //load local scores for this map
            SortedList<long, Result> scores = null;
            if (ScoreStore.Scores?.TryGetValue(info.uuid, out scores) ?? false)
            {
                Debug.Log(scores.Count);

                //show the local scores
                AddItemsToList(
                    scores.Count,
                    Instance.ScoreListItemPrefab,
                    (int index) =>
                      VerticalListPosition(index, Instance.ScoreListItemRect, Instance.ScoreListYOffset, Instance.ScoreListYSpacing) +
                      new Vector3(Instance.isStart ? 0 : 122.35f, 0, 0),
                    Instance.ScoreListContentTransform,
                    (GameObject songListItem, int index) =>
                    {
                        ScoreListItem scoreItem = songListItem.GetComponent<ScoreListItem>();
                        scoreItem.Load();
                        scoreItem.SetScore(scores.ElementAt(scores.Count - 1 - index).Value);
                    }
                );
            }
        }
    }

    public static void AddItemsToList(int count, GameObject prefab, Func<int, Vector3> getPosition, RectTransform list, Action<GameObject, int> initialize = null)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject songListItem = Instantiate(prefab, list);
            RectTransform rect = songListItem.GetComponent<RectTransform>();
            rect.localPosition = getPosition(i);
            initialize?.Invoke(songListItem, i);
        }
        RectTransform last = list.GetChild(list.childCount - 1).GetComponent<RectTransform>();
        RectTransform first = list.GetChild(0).GetComponent<RectTransform>();
        list.sizeDelta = new Vector2(list.sizeDelta.x, -last.localPosition.y + last.sizeDelta.y / 2 + first.rect.height / 2);
    }

    private static Vector3 VerticalListPosition(int index, Rect rect, int offset, int spacing)
    {
        return new Vector3(0, -offset - ((spacing + rect.height) * index) - rect.height / 2);
    }

    bool isStart = true;
    static bool first = true;

    public void Start()
    {
        ScoreStore.LoadOffline();

        if (BeatmapStore.Beatmaps == null)
            BeatmapStore.LoadAll();

        SongListItemRect = SongListItemPrefab.GetComponent<RectTransform>().rect;
        ScoreListItemRect = ScoreListItemPrefab.GetComponent<RectTransform>().rect;

        AddItemsToList(
            BeatmapStore.Beatmaps.Count,
            SongListItemPrefab,
            (int index) => VerticalListPosition(index, SongListItemRect, SongListYOffset, SongListYSpacing), 
            SongListContentTransform, 
            (GameObject songListItem, int index) =>
            {
                SongListItem item = songListItem.GetComponent<SongListItem>();
                item.Load();
                item.SetBeatmapInfo(BeatmapStore.Beatmaps[index]);
                item.SelectedChange(false);
            }
        );

        if (first)
            Selected = UnityEngine.Random.Range(0, BeatmapStore.Beatmaps.Count);
        else
            Selected = Selected;
        isStart = false;
        first = false;
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Initiate.Fade("MainMenuScene", Color.black, 3);
        }
    }
}
