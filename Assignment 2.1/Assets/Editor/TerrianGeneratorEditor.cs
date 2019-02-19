using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrianGenerator))]
public class TerrianGeneratorEditor : Editor {
    public override void OnInspectorGUI()
    {
        TerrianGenerator terrainGenerator = (TerrianGenerator)target;

        if (DrawDefaultInspector())
        {
            if (terrainGenerator.autoUpdate)
            {
                terrainGenerator.DrawTerrianInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.DrawTerrianInEditor();
        }
    }
}
