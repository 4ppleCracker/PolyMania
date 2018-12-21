using System.Collections;
using UnityEngine;
using Unity.Collections;
using System.Threading.Tasks;
using System;

[RequireComponent(typeof(AudioSource))]
public class Conductor : SingletonBehaviour<Conductor> {

    // Inspector fields
    public AudioClip Song => Player.clip;
    public AudioSource Player;

    private int m_extraDataLength = 0;
    public Time Position => Song == null ? new Time(0) : new Time(sec: (float)(Player.timeSamples - m_extraDataLength) / Song.frequency);
    public Time Length => Song == null ? new Time(0) : new Time(sec: (float)(Song.samples - m_extraDataLength) / Song.frequency);

    public bool SongLoaded => Song != null;

    public bool Playing => Player.isPlaying;

    public void Play(AudioClip song, int delay=0, int fadeTime=0)
    {
        StartCoroutine(PlayCoroutine(song, delay, fadeTime));
    }
    private IEnumerator PlayCoroutine(AudioClip song, int delay, int fadeTime)
    {
        //Add -delay- milliseconds of silence to start of song
        int extraData = song.frequency * song.channels * (delay / 1000);
        float[] songData = new float[song.samples * song.channels];
        song.GetData(songData, 0);
        song = AudioClip.Create(song.name, songData.Length + extraData, song.channels, song.frequency, false);
        float[] newSongData = new float[songData.Length + extraData];
        Array.Copy(songData, 0, newSongData, extraData, songData.Length);
        song.SetData(newSongData, 0);

        m_extraDataLength = extraData;

        Player.clip = song;
        if (Beatmap.CurrentlyLoaded.Notes[0].time.Ms < fadeTime) fadeTime = Beatmap.CurrentlyLoaded.Notes[0].time.Ms;
        StartCoroutine(Helper.FadeIn(Player, fadeTime / 1000));
        yield return null;
    }

    private void Update()
    {
        if (Position >= Length)
        {
            Player.Stop();
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
