using UnityEngine;
using UnityEditor;

public class Editor<T> : Editor where T : Object
{
    public new T target => (T)base.target;
}
