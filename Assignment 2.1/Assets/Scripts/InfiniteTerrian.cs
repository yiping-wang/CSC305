using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SimpInfo
{
    public int meshSimpFactor;
    public float visibleDistance;
}

public class InfiniteTerrian : MonoBehaviour {
    const float viewerMoveUpdateThreshold = 25f;
    const float sqrtViewerMoveUpdateThreshold = viewerMoveUpdateThreshold * viewerMoveUpdateThreshold;
    public static float maxVisibleDistance;
    public Transform viewer;
    public static Vector2 viewerPosition;
    public static Vector2 previousViewerPosition;
    int terrianSize;
    int terriansVisible;
    public Material material;
    TerrianGenerator terrianGenerator;
    Dictionary<Vector2, TerrianSpawn> terrianSpawnDict = new Dictionary<Vector2, TerrianSpawn>();
    List<TerrianSpawn> terrianSpawnsLastUpdate = new List<TerrianSpawn>();
    public SimpInfo[] simpFactors;

    private void Start()
    {
        maxVisibleDistance = simpFactors[simpFactors.Length - 1].visibleDistance;
        terrianGenerator = FindObjectOfType<TerrianGenerator>();
        terrianSize = 250;
        terriansVisible = Mathf.RoundToInt(maxVisibleDistance / terrianSize);

        UpdateTerrianSpawns();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        if ((previousViewerPosition - viewerPosition).sqrMagnitude > sqrtViewerMoveUpdateThreshold)
        {
            previousViewerPosition = viewerPosition;
            UpdateTerrianSpawns();
        }
    }

    void UpdateTerrianSpawns()
    {
        for(int i = 0; i < terrianSpawnsLastUpdate.Count; i++)
        {
            terrianSpawnsLastUpdate[i].SetVisible(false);
        }
        terrianSpawnsLastUpdate.Clear();

        int currSpawnCoordX = Mathf.RoundToInt(viewerPosition.x / terrianSize);
        int currSpawnCoordY = Mathf.RoundToInt(viewerPosition.y / terrianSize);

        for(int y = - terriansVisible; y <= terriansVisible; y++)
        {
            for (int x = - terriansVisible; x <= terriansVisible; x++)
            {
                Vector2 visibleTerrainPosition = new Vector2(currSpawnCoordX + x, currSpawnCoordY + y);
                if (terrianSpawnDict.ContainsKey(visibleTerrainPosition))
                {
                    terrianSpawnDict[visibleTerrainPosition].UpdateTerrianSpawn();
                    if (terrianSpawnDict[visibleTerrainPosition].IsVisible())
                    {
                        terrianSpawnsLastUpdate.Add(terrianSpawnDict[visibleTerrainPosition]);
                    }
                }
                else
                {
                    terrianSpawnDict.Add(visibleTerrainPosition, new TerrianSpawn(visibleTerrainPosition, terrianSize, maxVisibleDistance, transform, terrianGenerator, material, simpFactors));
                }
            }
        }
    }

    public class TerrianSpawn
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        float maxVisibleDistance;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        TerrianGenerator terrianGenerator;
        SimpInfo[] simpFactors;
        MeshSimp[] meshSimp;
        TerrianData terrianData;
        bool terrianDataReceived;
        int prevSimpIndex = -1;

        public TerrianSpawn(Vector2 coord, int terrianSize, float maxVisibleDistance, Transform parent, TerrianGenerator terrianGenerator, Material material, SimpInfo[] simpFactors)
        {
            this.simpFactors = simpFactors;
            this.terrianGenerator = terrianGenerator;
            this.maxVisibleDistance = maxVisibleDistance;
            position = coord * terrianSize;
            Vector3 worldPosition = new Vector3(position.x, 0, position.y);
            meshObject = new GameObject("Terrian");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshObject.transform.position = worldPosition;
            meshObject.transform.parent = parent;
            bounds = new Bounds(position, Vector2.one * terrianSize);
            SetVisible(false);
            meshSimp = new MeshSimp[simpFactors.Length];
            for (int i = 0; i < meshSimp.Length; i++)
            {
                meshSimp[i] = new MeshSimp(simpFactors[i].meshSimpFactor, terrianGenerator, UpdateTerrianSpawn);
            }
            this.terrianGenerator.RequestTerrianData(position, OnTerrianDataRecevied);
        }

        public void UpdateTerrianSpawn()
        {
            if (terrianDataReceived)
            {
                float minTerrianDistanceToViewer = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = minTerrianDistanceToViewer <= maxVisibleDistance;

                if (visible)
                {
                    int simpIndex = 0;
                    for (int i = 0; i < simpFactors.Length - 1; i++)
                    {
                        if (minTerrianDistanceToViewer > simpFactors[i].visibleDistance)
                        {
                            simpIndex += 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (simpIndex != prevSimpIndex)
                    {
                        MeshSimp meshSimpObj = meshSimp[simpIndex];
                        if (meshSimpObj.hasMesh)
                        {
                            prevSimpIndex = simpIndex;
                            meshFilter.mesh = meshSimpObj.mesh;
                        }
                        else if (!meshSimpObj.hasRequestedMesh)
                        {
                            meshSimpObj.RequestMesh(terrianData);
                        }
                    }
                }

                SetVisible(visible);
            }
        }

        public void SetVisible(bool v)
        {
            meshObject.SetActive(v);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

        void OnTerrianDataRecevied(TerrianData terrianData)
        {
            this.terrianData = terrianData;
            terrianDataReceived = true;
            Texture2D texture = TextureGenerator.GenerateTexture(terrianGenerator.terrianWidth, terrianGenerator.terrianLength, terrianData.color);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrianSpawn();
        }
    }
}
