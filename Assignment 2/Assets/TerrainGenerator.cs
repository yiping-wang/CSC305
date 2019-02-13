using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{

    public int Width = 250;
    public int Height = 250;
    public float Frequency = 10;
    public float Amplitude = 20;
    private float offsetX;
    private float offsetY;

    private Vector3[] vertices;
    private int[] triangles;

    private Mesh mesh;

    GameObject Plane;

    void Start()
    {
        Plane = gameObject;
        offsetX = Random.Range(0f, 1000f);
        offsetY = Random.Range(0f, 1000f);
        createGrid();
    }

    private void createGrid()
    {
        mesh = new Mesh();

        float[,] HeightMap = GenerateHeight();

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(Width) * (Height)];
        for (int i = 0, y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++, i++)
            {
                vertices[i] = new Vector3(x, HeightMap[x, y], y);
            }
        }

        mesh.vertices = vertices;

        triangles = new int[Width * Height * 6];
        for (int t = 0, v = 0, y = 0; y < Height - 1; y++, v++)
        {
            for (int x = 0; x < Width - 1; x++, t += 6, v++)
            {
                triangles[t] = v;
                triangles[t + 1] = v + Width;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + Width;
                triangles[t + 5] = v + Width + 1;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        Plane.GetComponent<MeshFilter>().mesh = mesh;
    }

    private float[,] GenerateHeight()
    {
        float[,] HeightMap = new float[Width, Height];
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                HeightMap[i, j] = Mathf.PerlinNoise((float)i / Width * Frequency, (float)j / Height * Frequency) * Amplitude;
            }
        }
        return HeightMap;
    }
}