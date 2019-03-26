using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDetailLevel
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int level;
    System.Action updateCallback;
    TerrianGenerator terrianGenerator;
    public GameObject[] gameObjects;

    public MeshDetailLevel(int level, System.Action updateCallback, TerrianGenerator terrianGenerator)
    {
        this.terrianGenerator = terrianGenerator;
        this.level = level;
        this.updateCallback = updateCallback;
    }

    void OnMeshDataReceived(MeshData meshData)
    {
        mesh = meshData.CreateMesh();
        hasMesh = true;
        updateCallback();
    }

    public void RequestMesh(TerrianData terrianData)
    {
        hasRequestedMesh = true;
        terrianGenerator.RequestMeshData(terrianData, level, OnMeshDataReceived);
    }
}