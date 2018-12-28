using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public static class Helper
{
    public static void SetBackgroundImage(Texture2D texture, float alpha = 1)
    {
        RawImage image = GameObject.Find("BackgroundImage").GetComponent<RawImage>();

        if (texture != null)
        {
            image.texture = texture;
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
    public static IEnumerable<T> Flatten<T>(IEnumerable<IEnumerable<T>> collectionList)
    {
        foreach(ICollection<T> collection in collectionList)
        {
            foreach(T obj in collection)
            {
                yield return obj;
            }
        }
    }
}
