using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolyMesh : SingletonBehaviour<PolyMesh>
{
    public int Count { get; private set; } = 6;
    public float Radius { get; private set; } = 3.5f;

    public const int MINIMUM_COUNT = 3;

    public Vector2[] SelectedTriangleUV => new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
    };
    public Vector2[] NonSelectedTriangleUV => new Vector2[]
    {
        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(1, 0),
    };

    private MeshFilter m_meshFilter;
    public MeshFilter meshFilter {
        get {
            if (m_meshFilter == null)
                return m_meshFilter = GetComponent<MeshFilter>();
            return m_meshFilter;
        }
    }
    private MeshRenderer m_meshRenderer;
    public MeshRenderer meshRenderer {
        get {
            if (m_meshRenderer == null)
                return m_meshRenderer = GetComponent<MeshRenderer>();
            return m_meshRenderer;
        }
    }

    private void Start()
    {
        Skin.CurrentlyLoadedSkin.Apply();
    }

    public void UpdateMesh()
    {
        Generate(Radius, Count);
    }

    public void Generate(float radius, int n)
    {
        if (n < MINIMUM_COUNT)
            throw new Exception("Cannot have less than 3 sides");

        Radius = radius;
        Count = n;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        //verticies
        Vector3[] verticies = new Vector3[n+1];
        for (int i = 0; i < n; i++)
        {
            float x = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
            float y = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
            verticies[i] = new Vector3(x, y, 0f);
        }
        verticies[n] = new Vector3(0, 0, 0);

        //triangles
        int[] triangles = new int[(n) * 3];
        for (int i = 0; i < n; i++)
        {
            triangles[i * 3] = n;
            triangles[(i * 3) + 1] = i;
            triangles[(i * 3) + 2] = (i + 1) % n;
        }

        //normals
        Vector3[] normals = new Vector3[verticies.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1), //<-- Triangle will be green
            new Vector2(1,0),
            new Vector2(0.5f, 0.5f),
        };

        //initialise
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.name = n + " sided polygon";
    }
}