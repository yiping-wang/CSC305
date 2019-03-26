using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (TerrianGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        TerrianGenerator mapGen = (TerrianGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawTerrianInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawTerrianInEditor();
        }
    }
}
