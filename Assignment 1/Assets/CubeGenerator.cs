/* 
UVic CSC 305, 2019 Spring
Assignment 01
Name:
UVic ID:

This is skeleton code we provided.
Feel free to add any member variables or functions that you need.
Feel free to modify the pre-defined function header or constructor if you need.
Please fill your name and uvic id.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment01
{
    public class CubeGenerator
    {
        Texture2D CubeResult;
        Vector3 A;
        Vector3 B;
        Vector3 C;
        Vector3 D;
        Vector3 E;
        Vector3 F;
        Vector3 RED = new Vector3(1, 0, 0);
        Vector3 GREEN = new Vector3(0, 1, 0);
        Vector3 BLUE = new Vector3(0, 0, 1);
        int CanvasWidth;
        int CanvasHeight;
        int imageSize = 1024;
        float ViewportWidth = 4;
        float ViewportHeight = 4;
        public CubeGenerator()
        {
            //you can define your cube vertices and indices in the constructor.
        }

        public Texture2D GenBarycentricVis(int width, int height)
        {
            /*
            implement ray-triangle intersection and 
            visualize the barycentric coordinate on each of the triangles of a cube, 
            with Red, Green and Blue for each coordinate.

            int width - width of the returned texture
            int height - height of the return texture
            return:
                Texture2D - Texture2D object which contains the rendered result
            */
            CubeResult = new Texture2D(width, height);
            CanvasHeight = height;
            CanvasWidth = width;
            Vector3 RayOrigin = new Vector3(0, 0, 0);

            B = new Vector3(0, -10, 10);
            D = new Vector3(0, 10, 10);
            A = new Vector3(-12, -9.5f, 10.5f);
            C = new Vector3(-12, 9.5f, 10.5f);
            E = new Vector3(12, 9.5f, 10.5f);
            F = new Vector3(12, -9.5f, 10.5f);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 RayDirection = Vector3.Normalize(new Vector3((-ViewportWidth / 2) + x * ViewportWidth / CanvasWidth, (-ViewportHeight / 2) + y * ViewportHeight / CanvasHeight, 1));
                    float t;
                    Vector3 BarycentricCoordinate;
                    if (IntersectTriangle(RayOrigin, RayDirection, A, C, B, out t, out BarycentricCoordinate) || 
                        IntersectTriangle(RayOrigin, RayDirection, B, C, D, out t, out BarycentricCoordinate) ||
                        IntersectTriangle(RayOrigin, RayDirection, B, D, E, out t, out BarycentricCoordinate) ||
                        IntersectTriangle(RayOrigin, RayDirection, B, E, F, out t, out BarycentricCoordinate)
                       )
                    {
                        Vector3 InterploationColor = BarycentricCoordinate.x * RED + BarycentricCoordinate.y * GREEN + BarycentricCoordinate.z * BLUE;
                        CubeResult.SetPixel(x, y, new Color(InterploationColor.x, InterploationColor.y, InterploationColor.z));
                    }
                    else 
                    {
                        CubeResult.SetPixel(x, y, Color.grey);
                    }
                }
            }

            CubeResult.Apply();
            return CubeResult;
        }

        public Texture2D GenUVMapping(int width, int height, Texture2D inputTexture)
        {
            /*
            implement UV mapping with the calculated barycentric coordinate in the previous step, 
            and visualize a texture image on each face of the cube.
            (choose any texture you like)
            we have declared textureOnCube as a public variable,
            you can attach texture to it from Unity.
            you can define your cube vertices and indices in this function.

            int width - width of the returned texture
            int height - height of the return texture
            Texture2D inputTexture - the texture you need to sample from
            return:
                Texture2D - Texture2D object which contains the rendered result
            */
            CubeResult = new Texture2D(width, height);

            B = new Vector3(0, -10, 10);
            D = new Vector3(0, 10, 10);
            A = new Vector3(-12, -9.5f, 10.5f);
            C = new Vector3(-12, 9.5f, 10.5f);
            E = new Vector3(12, 9.5f, 10.5f);
            F = new Vector3(12, -9.5f, 10.5f);

            CanvasHeight = height;
            CanvasWidth = width;

            Vector3 RayOrigin = new Vector3(0, 0, 0);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 RayDirection = Vector3.Normalize(new Vector3((-ViewportWidth / 2) + x * ViewportWidth / CanvasWidth, (-ViewportHeight / 2) + y * ViewportHeight / CanvasHeight, 1));
                    float t;
                    Vector3 BarycentricCoordinate;
                    if (IntersectTriangle(RayOrigin, RayDirection, A, C, B, out t, out BarycentricCoordinate))
                    {
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(Convert.ToInt32(imageSize * BarycentricCoordinate.z), Convert.ToInt32(imageSize * BarycentricCoordinate.y)));
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, D, C, B, out t, out BarycentricCoordinate))
                    {
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(Convert.ToInt32(imageSize - imageSize * BarycentricCoordinate.y), Convert.ToInt32(imageSize - imageSize * BarycentricCoordinate.z)));
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, D, E, B, out t, out BarycentricCoordinate))
                    {
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(Convert.ToInt32(imageSize - imageSize * BarycentricCoordinate.y), Convert.ToInt32(imageSize - imageSize * BarycentricCoordinate.z)));
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, F, E, B, out t, out BarycentricCoordinate))
                    {
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(Convert.ToInt32(imageSize * BarycentricCoordinate.z), Convert.ToInt32(imageSize * BarycentricCoordinate.y)));
                    }
                    else
                    {
                        CubeResult.SetPixel(x, y, Color.grey);
                    }
                }
            }

            CubeResult.Apply();
            return CubeResult;
        }

        private bool IntersectTriangle(Vector3 origin,
                                        Vector3 direction,
                                        Vector3 vA,
                                        Vector3 vB,
                                        Vector3 vC,
                                        out float t,
                                        out Vector3 barycentricCoordinate)
        {
            /*
            Vector3 origin - origin point of the ray
            Vector3 direction - the direction of the ray
            vA, vB, vC - 3 vertices of the target triangle
            out float t - distance the ray travelled to hit a point
            out Vector3 barycentricCoordinate - you should know what this is
            return:
                bool - indicating hit or not
            */
            float a = vA.x - vB.x;
            float b = vA.y - vB.y;
            float c = vA.z - vB.z;
            float d = vA.x - vC.x;
            float e = vA.y - vC.y;
            float f = vA.z - vC.z;
            float g = direction.x;
            float h = direction.y;
            float i = direction.z;
            float j = vA.x - origin.x;
            float k = vA.y - origin.y;
            float l = vA.z - origin.z;
            float M = a * (e * i - h * f) + b * (g * f - d * i) + c * (d * h - e * g);
            float beta = (j * (e * i - h * f) + k * (g * f - d * i) + l * (d * h - e * g)) / M;
            float gamma = (i * (a * k - j * b) + h * (j * c - a * l) + g * (b * l - k * c)) / M;
            if ((gamma < 0 || gamma > 1) || (beta < 0 || beta > 1) || (beta + gamma > 1))
            {
                t = 0;
                barycentricCoordinate = new Vector3(0, 0, 0);
                return false;
            }
            t = (f * (a * k - j * b) + e * (j * c - a * l) + d * (b * l - k * c)) / M;

            barycentricCoordinate = new Vector3(1 - gamma - beta, beta, gamma);
            return true;
        }
    }
}
