using UnityEditor;

[CustomEditor(typeof(AimController))]
public class AimControllerEditor : Editor<AimController>
{
    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Selected slice: ", target.SelectedSlice.ToString());
    }
}
