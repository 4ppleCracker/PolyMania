using System.Collections;
using UnityEngine;
using Unity.Collections;

[RequireComponent(typeof(AudioSource))]
public class Conductor : SingletonBehaviour<Conductor> {

    // Inspector fields
    public AudioClip Song;
    public AudioSource Player;

    public Time Position => new Time(sec: (float)Player.timeSamples / Song.frequency);
    public Time Length => new Time(sec: (float)Song.samples / Song.frequency);

    public bool SongLoaded => Song != null;

    public bool Playing = false;

    public void Play(AudioClip song)
    {
        Song = song;
        Player.Stop();
        Player.time = 0;
        Player.clip = Song;
        Player.Play();
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
