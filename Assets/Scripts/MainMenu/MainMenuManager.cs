public class MainMenuManager : SingletonBehaviour<MainMenuManager> {

	// Use this for initialization
	void Start () {
        if (ScoreStore.Scores != null)
            ScoreStore.LoadOffline();
        if (BeatmapStore.Beatmaps == null)
            BeatmapStore.LoadAll();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
