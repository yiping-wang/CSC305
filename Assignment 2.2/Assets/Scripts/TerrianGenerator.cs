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
    public TerrianAsset terrianAsset;
    public NoiseAsset noiseAsset;
    public TextureAsset textureAsset;
    public Material terrianMaterial;

	public bool autoUpdate;
	public TerrainType[] regions;

	Queue<DataThreadInfo<TerrianData>> terrianDataThreadInfoQueue = new Queue<DataThreadInfo<TerrianData>>();
	Queue<DataThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<DataThreadInfo<MeshData>>();

    void OnTextureUpdated()
    {
        textureAsset.ApplyToMaterial(terrianMaterial);
    }

    public void DrawTerrianInEditor() {
		TerrianData terrianData = GenerateTerrianData (Vector2.zero);
		TerrianDisplay display = FindObjectOfType<TerrianDisplay> ();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrianData.noise, terrianAsset.maxHeight, regions[1].height, detailLevel), TextureGenerator.TextureFromColorMap(terrianData.color, terrianSize));
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(terrianData.noise, terrianAsset.maxHeight, regions[1].height, spawnTerrianDetailLevel);
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
		float[,] noise = Noise.GenerateNoiseMap(terrianSize, noiseAsset.seed, noiseAsset.noiseScale, noiseAsset.octaves, noiseAsset.persistance, noiseAsset.lacunarity, center + noiseAsset.offset);
		Color[] color = new Color[terrianSize * terrianSize];
		for (int y = 0; y < terrianSize; y++) 
        {
			for (int x = 0; x < terrianSize; x++) 
            {
                float currentHeight = noise[x, y];
                Color regionColor = new Color();
                if (currentHeight < regions[1].height)
                {
                    regionColor = regions[0].color;
                }
                else if(currentHeight >= regions[1].height && currentHeight < regions[2].height)
                {
                    regionColor = regions[1].color;
                }
                else if (currentHeight >= regions[2].height && currentHeight < regions[3].height)
                {
                    regionColor = regions[2].color;
                }
                else
                {
                    regionColor = regions[3].color;
                }
                color[y * terrianSize + x] = regionColor;
			}
		}
        return new TerrianData(noise, color);
	}
}
