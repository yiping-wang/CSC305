using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSimp {
    public Mesh mesh;
    public bool hasMesh;
    public bool hasRequestedMesh;
    int meshSimplificationFactor;
    TerrianGenerator terrianGenerator;
    System.Action updateCallBack;

    public MeshSimp(int meshSimplificationFactor, TerrianGenerator terrianGenerator, System.Action updateCallBack)
    {
        this.updateCallBack = updateCallBack;
        this.terrianGenerator = terrianGenerator;
        this.meshSimplificationFactor = meshSimplificationFactor;
    }

    void OnMeshDataRecevied(MeshData meshData)
    {
        hasMesh = true;
        mesh = meshData.CreateMesh();
        updateCallBack();
    }
    
    public void RequestMesh(TerrianData terrianData)
    {
        hasRequestedMesh = true;
        terrianGenerator.RequestMeshData(terrianData, meshSimplificationFactor, OnMeshDataRecevied);
    }
}
