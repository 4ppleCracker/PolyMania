using System.Collections;
using UnityEngine;
using Unity.Collections;

[RequireComponent(typeof(AudioSource))]
public class Conductor : SingletonBehaviour<Conductor> {

    // Inspector fields
    public AudioClip Song;
    public AudioSource Player;

    public Time Position => new Time((float)Player.timeSamples / Song.frequency);
    public Time Length => new Time((float)Song.samples / Song.frequency);

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
	}
}
