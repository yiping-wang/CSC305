/* 
UVic CSC 305, 2019 Spring
Assignment 01
Name: Yiping Wang
UVic ID: V00894385
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment01
{
    public class NaiveRayTracer : MonoBehaviour
    {
        Texture2D renderedResult;
        public Texture2D textureOnCube;
        int canvasWidth;
        int canvasHeight;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Here is the Start function of class NaiveRayTracer");
            Camera thisCamera = gameObject.GetComponent<Camera>();
            Debug.Assert(thisCamera);
            canvasWidth = thisCamera.pixelWidth;
            canvasHeight = thisCamera.pixelHeight;
            Debug.Log("canvasWidth: " + canvasWidth);
            Debug.Log("canvasHeight: " + canvasHeight);
            renderedResult = new Texture2D(canvasWidth, canvasHeight);

            //following if-else statement helps graders to grade your assignment.
            //change the renderMethod to test your implementation.
            string renderMethod = "uvmapping";
            if (renderMethod == "checkboard")
            {
                CheckboardGenerator myRenderer = new CheckboardGenerator();
                renderedResult = myRenderer.GenCheckboard(canvasWidth, canvasHeight);
            }
            else if (renderMethod == "sphere")
            {
                SphereGenerator myRenderer = new SphereGenerator();
                renderedResult = myRenderer.GenSphere(canvasWidth, canvasHeight);
            }
            else if (renderMethod == "barycentric")
            {
                CubeGenerator myRenderer = new CubeGenerator();
                renderedResult = myRenderer.GenBarycentricVis(canvasWidth, canvasHeight);
            }
            else if (renderMethod == "uvmapping")
            {
                CubeGenerator myRenderer = new CubeGenerator();
                renderedResult = myRenderer.GenUVMapping(canvasWidth, canvasHeight, textureOnCube);
            }
            else
                throw new NotImplementedException();
            renderedResult.Apply();
        }

        //OnRenderImage is called after all rendering is complete to render image.
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(renderedResult, destination);
        }
    }
}