using UnityEngine;

//Currently does not properly work, need to change algorithm

public class AimController : SingletonBehaviour<AimController> {

    private int m_selectedSlice = 0;
    /// <summary>
    /// 0 indexed
    /// </summary>
    public int SelectedSlice { get { return m_selectedSlice; } private set { m_selectedSlice = value; PolyMesh.Instance.UpdateMesh(); } }

    public static bool PointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
        var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

        if ((s < 0) != (t < 0))
            return false;

        var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

        return A < 0 ?
                (s <= 0 && s + t >= A) :
                (s >= 0 && s + t <= A);
    }

    void Update () {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Triangle[] tris = PolyMesh.Instance.GetRealTriangles();
        for(int i = 0; i < tris.Length; i++)
        {
            Triangle tri = tris[i];
            if (PointInTriangle(mousePosition, tri.a, tri.b, tri.c))
            {
                SelectedSlice = i;
                break;
            }
        }
    }
}
