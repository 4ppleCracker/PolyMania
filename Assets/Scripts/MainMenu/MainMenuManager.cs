using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : SingletonBehaviour<MainMenuManager> {

    [SerializeField]
    Button PlayButton;

    static bool first = true;

	// Use this for initialization
	void Start () {
        if (first)
        {
            ScoreStore.LoadOffline();
            BeatmapStore.LoadAll();
        }
        first = true;

        PlayButton.onClick.AddListener(() =>
        {
            Initiate.Fade("SongSelectScene", Color.black, 2f);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
