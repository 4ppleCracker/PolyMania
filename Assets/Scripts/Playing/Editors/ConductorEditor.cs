using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Conductor))]
public class ConductorEditor : Editor<Conductor>
{
    bool showDebug = false;
    bool showNotes = false;

    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Separator();

        showDebug = EditorGUILayout.Foldout(showDebug, "Debug Info");
        if(showDebug)
        {
            EditorGUILayout.LabelField("  Playing: ", target.Playing.ToString());
            if (target.SongLoaded)
            {
                EditorGUILayout.LabelField("  Song Length: ", target.Length.ToString());
                if (Application.isPlaying)
                {
                    EditorGUILayout.LabelField("  Song Position: ", target.Position.ToString());
                }
            }
            if(Beatmap.CurrentlyLoaded != null)
            {
                showNotes = EditorGUILayout.Foldout(showNotes, "  Notes");
                if(showDebug)
                {
                    foreach(Note note in Beatmap.CurrentlyLoaded.Notes)
                    {
                        EditorGUILayout.LabelField("    " + note.ToString());
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
