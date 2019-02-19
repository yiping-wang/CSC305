using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class TerrianGenerator : MonoBehaviour {
    public bool autoUpdate;
    public int terrianWidth;
    public int terrianLength;
    public float persistance;
    public float lacunarity;
    public float noiseScale;
    public int octaves;
    public Vector2 offset;
    [Range(1, 3)]
    public int meshSimpFactor;
    float[,] terrianHeight;
    public int maxHeight;
    public TerrianType[] regions;
    float waterLevel;
    Queue<ThreadInfo<TerrianData>> terrianDataThreadInfoQueue = new Queue<ThreadInfo<TerrianData>>();
    Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

    public void DrawTerrianInEditor()
    {
        TerrianData terrianData = GenerateTerrian(Vector2.zero);
        TerrianDisplay terrianDisplay = FindObjectOfType<TerrianDisplay>();
        Texture2D texture = TextureGenerator.GenerateTexture(terrianWidth, terrianLength, terrianData.color);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(terrianData.height, maxHeight, waterLevel, meshSimpFactor);
        terrianDisplay.DrawMesh(meshData, texture);
    }

    public void RequestTerrianData(Vector2 center, Action<TerrianData> callback)
    {
        ThreadStart threadStart = delegate
        {
            TerrianDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    public void RequestMeshData(TerrianData terrianData, int meshSimp, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(terrianData, meshSimp, callback);
        };

        new Thread(threadStart).Start();
    }

    void TerrianDataThread(Vector2 center, Action<TerrianData> callback)
    {
        TerrianData terrianData = GenerateTerrian(center);
        lock(terrianDataThreadInfoQueue)
        {
            terrianDataThreadInfoQueue.Enqueue(new ThreadInfo<TerrianData>(callback, terrianData));
        }
    }

    void MeshDataThread(TerrianData terrianData, int meshSimp, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(terrianData.height, maxHeight, waterLevel, meshSimp);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if(terrianDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < terrianDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<TerrianData> threadInfo = terrianDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public TerrianData GenerateTerrian(Vector2 center)
    {
        waterLevel = regions[0].level;
        terrianHeight = NoiseGenerator.GenerateTerrainLevel(terrianWidth, terrianLength, noiseScale, octaves, persistance, lacunarity, offset+center);
        Color[] terrianColorByHeight = new Color[terrianWidth * terrianLength]; 
        for (int y = 0; y < terrianLength; y++)
        {
            for (int x = 0; x < terrianWidth; x++)
            {
                float currentHeight = terrianHeight[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].level)
                    {
                        terrianColorByHeight[y * terrianWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return new TerrianData(terrianHeight, terrianColorByHeight);
    }
}
