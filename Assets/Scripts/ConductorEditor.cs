using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Conductor))]
public class ConductorEditor : Editor<Conductor>
{
    SerializedProperty songProperty;
    bool showDebug = false;

    public void OnEnable()
    {
        songProperty = serializedObject.FindProperty("Song");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(songProperty);

        EditorGUILayout.Separator();

        showDebug = EditorGUILayout.Foldout(showDebug, "Debug Info");
        if(showDebug)
        {
            if (target.SongLoaded)
            {
                EditorGUILayout.LabelField("  Song Length: ", target.Length.ToString());
                if (Application.isPlaying)
                {
                    EditorGUILayout.LabelField("  Song Position: ", target.Position.ToString());
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        Repaint();
    }
}
