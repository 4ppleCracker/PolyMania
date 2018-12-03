using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(NoteObject))]
public class NoteObjectEditor : Editor<NoteObject>
{
    private void OnSceneGUI()
    {
        Handles.Label(target.transform.position + Vector3.up * 1.1f, "Rotation: " + target.GetDegrees());
        Handles.Label(target.transform.position + Vector3.up * 1.2f, "Slice: " + target.note.slice);
        Handles.Label(target.transform.position + Vector3.up * 1.3f, "Time to click: " + target.note.TimeToClick);
        Handles.Label(target.transform.position + Vector3.up * 1.4f, "Time: " + target.note.time);
    }
}
#endif