using UnityEditor;

[CustomEditor(typeof(PolyMesh))]
public class PolyMeshEditor : Editor<PolyMesh>
{
    int count;
    float radius;
    private void OnEnable()
    {
        count = PolyMesh.Instance.Count;
        radius = PolyMesh.Instance.Radius;
        PolyMesh.Instance.Generate(radius, count);
    }
    public override void OnInspectorGUI()
    {
        if (count < PolyMesh.MINIMUM_COUNT) count = PolyMesh.MINIMUM_COUNT;

        int _count = EditorGUILayout.IntField("Count", count = PolyMesh.Instance.Count);
        float _radius = EditorGUILayout.FloatField("Radius", radius = PolyMesh.Instance.Radius);

        if (_count != count || _radius != radius)
        {
            count = _count;
            radius = _radius;
            PolyMesh.Instance.Generate(radius, count);
        }
    }
}
