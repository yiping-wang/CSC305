using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{

    public int GridWidth = 250;
    public int GridHeight = 250;
    public float LakeLevel = 0.4f;
    public float Frequency = 4;
    public float Amplitude = 20;
    float offsetX;
    float offsetY;
    Mesh mesh;
    Camera Camera;
    GameObject Plane;

    void Start()
    {
        offsetX = Random.Range(0f, 1000f);
        offsetY = Random.Range(0f, 1000f);
        Plane = gameObject;
        Camera = Camera.main;
        Plane.transform.position = new Vector3(-125, -10, 100);
        Camera.transform.position = new Vector3(0, 250, -130);
        Camera.transform.localEulerAngles = new Vector3(30, 0, 0);

        CreateMesh();
    }

    private void CreateMesh()
    {
        Vector3[] vertices;
        int[] triangles;
        mesh = new Mesh();
        float[,] HeightMap = GenerateHeight();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(GridWidth) * (GridHeight)];
        for (int i = 0, y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++, i++)
            {
                vertices[i] = new Vector3(x, HeightMap[x, y], y);
            }
        }
        mesh.vertices = vertices;

        triangles = new int[GridWidth * GridHeight * 6];
        for (int t = 0, v = 0, y = 0; y < GridHeight - 1; y++, v++)
        {
            for (int x = 0; x < GridWidth - 1; x++, t += 6, v++)
            {
                triangles[t] = v;
                triangles[t + 1] = v + GridWidth;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + GridWidth;
                triangles[t + 5] = v + GridWidth + 1;
            }
        }
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        Plane.GetComponent<MeshFilter>().mesh = mesh;
    }

    private float[,] GenerateHeight()
    {
        float[,] HeightMap = new float[GridWidth, GridHeight];
        for (int j = 0; j < GridHeight; j++)
        {
            for (int i = 0; i < GridWidth; i++)
            {
                HeightMap[i, j] = Mathf.PerlinNoise((float)i / GridWidth * Frequency + offsetX, (float)j / 
                                                    GridHeight * Frequency + offsetY) * Amplitude;
                if (HeightMap[i, j] < LakeLevel * Amplitude) {
                    HeightMap[i, j] = LakeLevel * Amplitude;
                }
            }
        }
        return HeightMap;
    }
}