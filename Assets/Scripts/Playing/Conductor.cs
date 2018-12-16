using System.Collections;
using UnityEngine;
using Unity.Collections;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class Conductor : SingletonBehaviour<Conductor> {

    // Inspector fields
    public AudioClip Song => Player.clip;
    public AudioSource Player;

    public Time Position => Song == null ? new Time(0) : new Time(sec: (float)Player.timeSamples / Song.frequency);
    public Time Length => Song == null ? new Time(0) : new Time(sec: (float)Song.samples / Song.frequency);

    public bool SongLoaded => Song != null;

    public bool Playing = false;

    public void Play(AudioClip song, int delay=0)
    {
        Player.time = 0;
        Player.clip = song;
        if (Beatmap.CurrentlyLoaded.Notes[0].time.Ms < delay) delay = Beatmap.CurrentlyLoaded.Notes[0].time.Ms;
        StartCoroutine(Helper.FadeIn(Player, delay / 1000));
        Playing = true;
    }

    private void Update()
    {
        if (Position >= Length)
        {
            Playing = false;
        }
    }

    /// <param name="beatType">1 = whole, 2 = half, 4 = quarter, etc</param>
    public static Time BeatsToTime(int beatNum, float bpm, int beatType)
    {
        float bps = bpm / 60;
        return new Time(sec: beatNum / bps * 4 / beatType);
    }

    // Use this for initialization
    void Start ()
    {
        // Get the components
        Player = GetComponent<AudioSource>();
	}
}
