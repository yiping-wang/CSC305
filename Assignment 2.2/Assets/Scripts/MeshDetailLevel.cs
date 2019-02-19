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
    MapGenerator mapGenerator;

    public MeshDetailLevel(int level, System.Action updateCallback, MapGenerator mapGenerator)
    {
        this.mapGenerator = mapGenerator;
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
        mapGenerator.RequestMeshData(terrianData, level, OnMeshDataReceived);
    }
}