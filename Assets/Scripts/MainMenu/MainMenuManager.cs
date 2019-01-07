using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : SingletonBehaviour<MainMenuManager> {

    [SerializeField]
    Button PlayButton;

	// Use this for initialization
	void Start () {
        if (ScoreStore.Scores != null)
            ScoreStore.LoadOffline();
        if (BeatmapStore.Beatmaps == null)
            BeatmapStore.LoadAll();

        PlayButton.onClick.AddListener(() =>
        {
            Initiate.Fade("SongSelectScene", Color.black, 2f);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
