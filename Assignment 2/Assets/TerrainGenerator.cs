using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{

    public int Width = 250;
    public int Height = 250;
    public float scale = 20;

    private Vector3[] vertices;
    private int[] triangles;

    private Mesh mesh;

    GameObject Plane;

    void Start()
    {
        Plane = gameObject;
        createGrid();
    }

    private void createGrid()
    {
        mesh = new Mesh();
        System.Random rand = new System.Random();

        float[,] HeightMap = GenerateHeight();

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(Width + 1) * (Height + 1)];
        for (int i = 0, y = 0; y <= Height; y++)
        {
            for (int x = 0; x <= Width; x++, i++)
            {
                vertices[i] = new Vector3(x, HeightMap[x, y] * 10, y);
            }
        }

        mesh.vertices = vertices;

        triangles = new int[Width * Height * 6];
        for (int t = 0, v = 0, y = 0; y < Height; y++, v++)
        {
            for (int x = 0; x < Width; x++, t = t + 6, v++)
            {
                triangles[t] = v;
                triangles[t + 3] = triangles[t + 2] = v + 1;
                triangles[t + 4] = triangles[t + 1] = v + Width + 1;
                triangles[t + 5] = v + Width + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        Plane.GetComponent<MeshFilter>().mesh = mesh;
    }

    private float[,] GenerateHeight()
    {
        float[,] height = new float[Width + 1, Height + 1];
        for (int i = 0; i <= Width; i++)
        {
            for (int j = 0; j <= Height; j++)
            {
                height[i, j] = CalculateHeight(i, j);
            }
        }
        return height;
    }

    private float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / Width * scale;
        float yCoord = (float)y / Height * scale;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }

}