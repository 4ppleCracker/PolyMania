using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Beatmap))]
public class BeatmapEditor : Editor<Beatmap>
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        EditorGUILayout.LabelField("Metadata");
        {
            target.SongName = EditorGUILayout.TextField("Song Name", target.SongName);
        }

        EditorGUILayout.LabelField("Modifiers");
        {
            target.Bpm = EditorGUILayout.IntField("Bpm", target.Bpm);

            target.SliceCount = (uint)EditorGUILayout.IntField("Slice Count", (int)target.SliceCount);
            if (target.SliceCount < PolyMesh.MINIMUM_COUNT)
                target.SliceCount = PolyMesh.MINIMUM_COUNT;

            target.AccMod = EditorGUILayout.FloatField("Accuracy", target.AccMod);

            target.SpeedMod = EditorGUILayout.FloatField("Speed", target.SpeedMod);
        }

        EditorGUILayout.LabelField("Map Data");
        {
            target.BackgroundImage = (Texture2D)EditorGUILayout.ObjectField("Image", target.BackgroundImage, typeof(Texture2D), false);
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Notes"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif