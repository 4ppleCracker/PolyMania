using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}