using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Helper
{
    public static void SetBackgroundImage(Texture2D texture)
    {
        Image image = GameObject.Find("BackgroundImage").GetComponent<Image>();

        if (texture != null)
        {
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.color = Color.white;
        } else
        {
            image.color = Color.clear;
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime, Action after=null)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * UnityEngine.Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.Stop();

        after?.Invoke();
    }
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime, Action after = null)
    {
        audioSource.volume = 0.0f;

        audioSource.Play();

        while (audioSource.volume < 1)
        {
            audioSource.volume += 1 * UnityEngine.Time.deltaTime / FadeTime;

            yield return null;
        }

        after?.Invoke();
    }
}
