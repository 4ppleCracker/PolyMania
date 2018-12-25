using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public static class Helper
{
    public static void SetBackgroundImage(Texture2D texture, float alpha = 1)
    {
        Image image = GameObject.Find("BackgroundImage").GetComponent<Image>();

        if (texture != null)
        {
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            Color color = Color.white;
            color.a = alpha;
            image.color = color;
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
    public static string SanitizeString(string str)
    {
        return new string(str.Where(char.IsLetterOrDigit).ToArray());
    }
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }
}
