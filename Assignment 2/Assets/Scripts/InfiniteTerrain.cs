using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct LODInfo
{
    public int lod;
    public float visibleDstThreshold;
}

public class InfiniteTerrain : MonoBehaviour {
    public GameObject[] trees;

	const float viewerMoveDistanceForUpdatingTerrian = 25f;
	const float squareViewerMoveDistanceForUpdatingTerrian = viewerMoveDistanceForUpdatingTerrian * viewerMoveDistanceForUpdatingTerrian;

	public LODInfo[] detailLevels;
	public static float maxVisibleDistance;

	public Transform viewer;
	public Material terrianMaterial;

	public static Vector2 viewerPosition;
	Vector2 prevViewPosition;

	static TerrianGenerator terrianGenerator;
	int terrianSize;
	int chunksVisibleInViewDst;

    Dictionary<Vector2, SpawnTerrian> terrainChunkDictionary = new Dictionary<Vector2, SpawnTerrian>();
    static List<SpawnTerrian> terrainChunksVisibleLastUpdate = new List<SpawnTerrian>();

	void Start() 
    {
        terrianGenerator = FindObjectOfType<TerrianGenerator>();

        maxVisibleDistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		terrianSize = TerrianGenerator.terrianSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxVisibleDistance / terrianSize);

        UpdateVisibleChunks();
	}

	void Update() 
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

		if ((prevViewPosition - viewerPosition).sqrMagnitude > squareViewerMoveDistanceForUpdatingTerrian) 
        {
			prevViewPosition = viewerPosition;
            UpdateVisibleChunks();
		}
	}
		
	void UpdateVisibleChunks() 
    {
		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) 
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
		}
        terrainChunksVisibleLastUpdate.Clear();
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / terrianSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / terrianSize);

		for (int yOffset = - chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) 
        {
			for (int xOffset = - chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) 
            {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) 
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainSpawn();
				} 
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new SpawnTerrian(viewedChunkCoord, terrianSize, detailLevels, transform, terrianMaterial, trees));
				}
			}
		}
	}

	public class SpawnTerrian
    {
		GameObject meshObject;
        GameObject[] trees;
		Vector2 terrianPosition;
		Bounds bounds;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
        MeshCollider meshCollider;
		LODInfo[] detailLevels;
		MeshDetailLevel[] meshDetailLevel;
		TerrianData terrianData;
		bool terrianDataReceived;
		int previousDetailLevelIndex = -1;

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

		public SpawnTerrian(Vector2 terrianCoord, int terrianSize, LODInfo[] detailLevels, Transform parent, Material material, GameObject[] trees) 
        {
            this.trees = trees;
			this.detailLevels = detailLevels;
            terrianPosition = terrianCoord * terrianSize;
            bounds = new Bounds(terrianPosition, Vector2.one * terrianSize);
            Vector3 worldPosition = new Vector3(terrianPosition.x, 0, terrianPosition.y);
			meshObject = new GameObject("Terrain");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
			meshRenderer.material = material;
            meshObject.transform.position = worldPosition;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one;
			SetVisible(false);
			meshDetailLevel = new MeshDetailLevel[detailLevels.Length];
			for (int i = 0; i < detailLevels.Length; i++)
            {
				meshDetailLevel[i] = new MeshDetailLevel(detailLevels[i].lod, UpdateTerrainSpawn, terrianGenerator);
			}
            terrianGenerator.RequestTerrianData(terrianPosition, OnTerrianDataReceived);
		}

		void OnTerrianDataReceived(TerrianData terrianData)
        {
			this.terrianData = terrianData;
			terrianDataReceived = true;

            UpdateTerrainSpawn();
		}

        void GenerateTrees(TerrianData terrianData, Mesh mesh)
        {
            if (mesh != null)
            {
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                System.Random rand = new System.Random();
                int index = 0;
                foreach(Vector3 vertex in vertices)
                {
                    if (vertex.y > 70 && vertex.y < 75)
                    {
                        var tree = Instantiate(trees[rand.Next(0,3)], new Vector3(vertex.x + terrianPosition.x, vertex.y, vertex.z + terrianPosition.y), Quaternion.identity);
                        tree.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        tree.transform.rotation = Quaternion.FromToRotation(Vector3.up, normals[index]);
                        tree.transform.parent = meshObject.transform;
                    }
                    index++;
                }
            }
        }

		public void UpdateTerrainSpawn()
        {
			if (terrianDataReceived) 
            {
                float terrianToViewer = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = terrianToViewer <= maxVisibleDistance;

				if (visible) 
                {
					int detailLevelIndex = 0;

					for (int i = 0; i < detailLevels.Length - 1; i++) 
                    {
                        if (terrianToViewer > detailLevels[i].visibleDstThreshold) 
                        {
                            detailLevelIndex = i + 1;
						} 
                        else 
                        {
							break;
						}
					}
                    if (detailLevelIndex != previousDetailLevelIndex) 
                    {
                        MeshDetailLevel meshDetailLevelObj = meshDetailLevel[detailLevelIndex];
                        if (meshDetailLevelObj.hasMesh) 
                        {
                            previousDetailLevelIndex = detailLevelIndex;
                            meshFilter.mesh = meshDetailLevelObj.mesh;
                            meshCollider.sharedMesh = meshDetailLevelObj.mesh;
						} 
                        else if (!meshDetailLevelObj.hasRequestedMesh)
                        {
                            meshDetailLevelObj.RequestMesh(terrianData);
						}

                        GenerateTrees(terrianData, meshDetailLevelObj.mesh);
					}

                    terrainChunksVisibleLastUpdate.Add(this);
				}
                SetVisible(visible);
			}
		}
	}
}
