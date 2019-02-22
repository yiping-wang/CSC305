using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TerrianGenerator : MonoBehaviour {
	public const int terrianSize = 241;
	[Range(0,6)]
	public int detailLevel;
    public TerrianAsset terrianAsset;
    public NoiseAsset noiseAsset;
    public TextureAsset textureAsset;
    public Material terrianMaterial;
	public bool autoUpdate;

	Queue<DataThreadInfo<TerrianData>> terrianDataThreadInfoQueue = new Queue<DataThreadInfo<TerrianData>>();
	Queue<DataThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<DataThreadInfo<MeshData>>();

    void OnTextureUpdated()
    {
        textureAsset.ApplyToMaterial(terrianMaterial);
    }

    void OnValueUpdate()
    {
        if (!Application.isPlaying)
        {
            DrawTerrianInEditor();
        }
    }

    public void DrawTerrianInEditor() {
		TerrianData terrianData = GenerateTerrianData (Vector2.zero);
		TerrianDisplay display = FindObjectOfType<TerrianDisplay> ();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(terrianData.noise, terrianAsset.maxHeight, terrianAsset.waterLevel, detailLevel));
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(terrianData.noise, terrianAsset.maxHeight, terrianAsset.waterLevel, spawnTerrianDetailLevel);
		lock (meshDataThreadInfoQueue) 
        {
            meshDataThreadInfoQueue.Enqueue(new DataThreadInfo<MeshData>(callback, meshData));
		}
	}

	void Update() {
        Light sun = FindObjectOfType<Light>();
        textureAsset.UpdateLight(terrianMaterial, sun.transform.position, sun.intensity);

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
        return new TerrianData(noise);
	}

    private void OnValidate()
    {
        if (terrianAsset != null)
        {
            terrianAsset.OnValuesUpdated -= OnValueUpdate;
            terrianAsset.OnValuesUpdated += OnValueUpdate;
        }
        if (noiseAsset != null)
        {
            noiseAsset.OnValuesUpdated -= OnValueUpdate;
            noiseAsset.OnValuesUpdated += OnValueUpdate;
        }
        if (textureAsset != null)
        {
            textureAsset.OnValuesUpdated -= OnTextureUpdated;
            textureAsset.OnValuesUpdated += OnTextureUpdated;
        }
    }
}
