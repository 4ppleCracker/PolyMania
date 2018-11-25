﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Conductor))]
public class ConductorEditor : Editor<Conductor>
{
    SerializedProperty songProperty;
    bool showDebug = false;
    bool showNotes = false;

    public void OnEnable()
    {
        songProperty = serializedObject.FindProperty("Song");
    }

    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(songProperty);

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