using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class Editor<T> : Editor where T : Object
{
    public new T target => (T)base.target;
}
#endif
