using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T instance;
    public static T Instance {
        get {
            if(instance == null || instance.name == "null")
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                    instance = CreateMe();
            }
            return instance;
        }
    }
    protected static T CreateMe() => instance = new GameObject(nameof(T), typeof(T)).GetComponent<T>();
}
