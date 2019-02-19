using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public class TerrianGenerator : MonoBehaviour {
	public const int terrianSize = 241;
	[Range(0,6)]
	public int detailLevel;
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float maxHeight;
	public bool autoUpdate;
	public TerrainType[] regions;

	Queue<DataThreadInfo<TerrianData>> terrianDataThreadInfoQueue = new Queue<DataThreadInfo<TerrianData>>();
	Queue<DataThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<DataThreadInfo<MeshData>>();


    public void DrawTerrianInEditor() {
		TerrianData terrianData = GenerateTerrianData (Vector2.zero);
		TerrianDisplay display = FindObjectOfType<TerrianDisplay> ();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrianData.noise, maxHeight, regions[1].height, detailLevel), TextureGenerator.TextureFromColorMap(terrianData.color, terrianSize));
	}

	public void RequestTerrianData(Vector2 center, Action<TerrianData> callback) 
    {
		ThreadStart threadStart = delegate 
        {
			TerrianDataThread (center, callback);
		};
        new Thread(threadStart).Start();
	}

	void TerrianDataThread(Vector2 center, Action<TerrianData> callback) {
        TerrianData terrianData = GenerateTerrianData(center);
		lock (terrianDataThreadInfoQueue) 
        {
            terrianDataThreadInfoQueue.Enqueue(new DataThreadInfo<TerrianData>(callback, terrianData));
		}
	}

	public void RequestMeshData(TerrianData terrianData, int spawnTerrianDetailLevel, Action<MeshData> callback) {
		ThreadStart threadStart = delegate 
        {
            MeshDataThread(terrianData, spawnTerrianDetailLevel, callback);
		};
        new Thread(threadStart).Start();
	}

	void MeshDataThread(TerrianData terrianData, int spawnTerrianDetailLevel, Action<MeshData> callback) 
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(terrianData.noise, maxHeight, regions[1].height, spawnTerrianDetailLevel);
		lock (meshDataThreadInfoQueue) 
        {
            meshDataThreadInfoQueue.Enqueue(new DataThreadInfo<MeshData>(callback, meshData));
		}
	}

	void Update() {
		if (terrianDataThreadInfoQueue.Count > 0) 
        {
			for (int i = 0; i < terrianDataThreadInfoQueue.Count; i++) 
            {
                DataThreadInfo<TerrianData> threadInfo = terrianDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0) 
        {
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) 
            {
                DataThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callback (threadInfo.parameter);
			}
		}
	}

	TerrianData GenerateTerrianData(Vector2 center) {
		float[,] noise = Noise.GenerateNoiseMap(terrianSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset);

		Color[] color = new Color[terrianSize * terrianSize];
		for (int y = 0; y < terrianSize; y++) 
        {
			for (int x = 0; x < terrianSize; x++) 
            {
                float currentHeight = noise[x, y];
				for (int i = 0; i < regions.Length; i++) 
                {
					if (currentHeight >= regions [i].height) 
                    {
                        color[y * terrianSize + x] = regions[i].color;
					} 
                    else 
                    {
						break;
					}
				}
			}
		}

        return new TerrianData(noise, color);
	}
}
