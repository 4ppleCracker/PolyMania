using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyMesh : SingletonBehaviour<PolyMesh> {

    public int Count { get; private set; }
    public float Radius { get; private set; }

    public const int MINIMUM_COUNT = 3;

    public void Generate(float radius, int n)
    {
        if (n < MINIMUM_COUNT)
            throw new Exception("Cannot have less than 3 sides");

        Radius = radius;
        Count = n;

        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        //verticies
        Vector3[] verticies = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            float x = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
            float y = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
            verticies[i] = new Vector3(x, y, 0f);
        }

        //triangles
        List<int> trianglesList = new List<int> { };
        for (int i = 0; i < (n - 2); i++)
        {
            trianglesList.Add(0);
            trianglesList.Add(i + 1);
            trianglesList.Add(i + 2);
        }
        int[] triangles = trianglesList.ToArray();

        //normals
        Vector3[] normals = new Vector3[verticies.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        //initialise
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.name = "PolyMesh";
    }
}
