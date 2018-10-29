using System.Collections;
using UnityEngine;
using Unity.Collections;

[RequireComponent(typeof(AudioSource))]
public class Conductor : SingletonBehaviour<Conductor> {

    // Inspector fields
    public AudioClip Song;
    public AudioSource Player;

    public float Seconds => (float)Player.timeSamples / Song.frequency;
    public int Position => Player.timeSamples;
    public int Length => Song.samples;

    public bool SongLoaded => Song != null;

    public void Play()
    {
        Player.Stop();
        Player.clip = Song;
        Player.Play();
    }

    // Use this for initialization
    void Start () {
        // Get the components
        Player = GetComponent<AudioSource>();

        Play();
	}

	// Update is called once per frame
	void Update () {
        if (Beatmap.CurrentlyLoaded.AnyNotesLeft)
        {
            Note note;
            if (Position >= (note = Beatmap.CurrentlyLoaded.Notes.Peek()).time)
            {
                Debug.Log(note.slice);
            }
        }
    }
}
