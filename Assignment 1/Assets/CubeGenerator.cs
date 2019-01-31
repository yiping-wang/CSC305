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
    public class CubeGenerator
    {
        Texture2D CubeResult;
        Vector3 RayOrigin;
        Vector3 Red;
        Vector3 Green;
        Vector3 Blue;
        Vector3 VertexA;
        Vector3 VertexB;
        Vector3 VertexC;
        Vector3 VertexD;
        Vector3 VertexE;
        Vector3 VertexF;
        float ViewportWidth;
        float ViewportHeight;
        int CanvasWidth;
        int CanvasHeight;

        public CubeGenerator()
        {
            RayOrigin = new Vector3(0, 0, 0);
            Red = new Vector3(1, 0, 0);
            Green = new Vector3(0, 1, 0);
            Blue = new Vector3(0, 0, 1);

            // Unity Display 1920 * 1680
            VertexA = new Vector3(-12, -9.5f, 10.5f);
            VertexB = new Vector3(0, -10, 10);
            VertexC = new Vector3(-12, 9.5f, 10.5f);
            VertexD = new Vector3(0, 10, 10);
            VertexE = new Vector3(12, 9.5f, 10.5f);
            VertexF = new Vector3(12, -9.5f, 10.5f);
            ViewportWidth = 4;
            ViewportHeight = 4;
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

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 RayDirection = Vector3.Normalize(new Vector3((-ViewportWidth / 2) + x * ViewportWidth / CanvasWidth, (-ViewportHeight / 2) + y * ViewportHeight / CanvasHeight, 1));
                    float t;
                    Vector3 barycentricCoordinate;
                    if (IntersectTriangle(RayOrigin, RayDirection, VertexA, VertexC, VertexB, out t, out barycentricCoordinate) || 
                        IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexC, VertexD, out t, out barycentricCoordinate) ||
                        IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexD, VertexE, out t, out barycentricCoordinate) ||
                        IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexE, VertexF, out t, out barycentricCoordinate)
                       )
                    {
                        Vector3 interploatedColor = barycentricCoordinate.x * Red + barycentricCoordinate.y * Green + barycentricCoordinate.z * Blue;
                        CubeResult.SetPixel(x, y, new Color(interploatedColor.x, interploatedColor.y, interploatedColor.z));
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
            int imageWidth = inputTexture.width;
            CubeResult = new Texture2D(width, height);
            CanvasHeight = height;
            CanvasWidth = width;

            Vector2 u0 = new Vector2(0, 0);
            Vector2 u1 = new Vector2(imageWidth, 0);
            Vector2 v0 = new Vector2(0, imageWidth);
            Vector2 v1 = new Vector2(imageWidth, imageWidth);
                
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 RayDirection = Vector3.Normalize(new Vector3((-ViewportWidth / 2) + x * ViewportWidth / CanvasWidth, (-ViewportHeight / 2) + y * ViewportHeight / CanvasHeight, 1));
                    float t;
                    Vector3 barycentricCoordinate;
                    if (IntersectTriangle(RayOrigin, RayDirection, VertexA, VertexC, VertexB, out t, out barycentricCoordinate))
                    {
                        Vector2 uv = GetUVCoordinate(u0, v0, u1, barycentricCoordinate);
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(Convert.ToInt32(uv.x), Convert.ToInt32(uv.y)));
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexC, VertexD, out t, out barycentricCoordinate))
                    {
                        Vector2 uv = GetUVCoordinate(u1, v0, v1, barycentricCoordinate);
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(Convert.ToInt32(uv.x), Convert.ToInt32(uv.y)));
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexD, VertexE, out t, out barycentricCoordinate))
                    {
                        Vector2 uv = GetUVCoordinate(u0, v0, v1, barycentricCoordinate);
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(imageWidth - Convert.ToInt32(uv.x), Convert.ToInt32(uv.y)));
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexE, VertexF, out t, out barycentricCoordinate))
                    {
                        Vector2 uv = GetUVCoordinate(u0, v1, u1, barycentricCoordinate);
                        CubeResult.SetPixel(x, y, inputTexture.GetPixel(imageWidth - Convert.ToInt32(uv.x), Convert.ToInt32(uv.y)));
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

        private bool IntersectTriangle(Vector3 rayOrigin,
                                        Vector3 rayDirection,
                                        Vector3 vertexA,
                                        Vector3 vertexB,
                                        Vector3 vertexC,
                                        out float t,
                                        out Vector3 barycentricCoordinate)
        {
            /*
            Vector3 rayOrigin - origin point of the ray
            Vector3 rayDirection - the direction of the ray
            vertexA, vertexB, vertexC - 3 vertices of the target triangle
            out float t - distance the ray travelled to hit a point
            out Vector3 barycentricCoordinate - you should know what this is
            return:
                bool - indicating hit or not
            */
            float a = vertexA.x - vertexB.x;
            float b = vertexA.y - vertexB.y;
            float c = vertexA.z - vertexB.z;
            float d = vertexA.x - vertexC.x;
            float e = vertexA.y - vertexC.y;
            float f = vertexA.z - vertexC.z;
            float g = rayDirection.x;
            float h = rayDirection.y;
            float i = rayDirection.z;
            float j = vertexA.x - rayOrigin.x;
            float k = vertexA.y - rayOrigin.y;
            float l = vertexA.z - rayOrigin.z;
            float M = a * (e * i - h * f) + b * (g * f - d * i) + c * (d * h - e * g);
            float beta = (j * (e * i - h * f) + k * (g * f - d * i) + l * (d * h - e * g)) / M;
            float gamma = (i * (a * k - j * b) + h * (j * c - a * l) + g * (b * l - k * c)) / M;
            if (gamma < 0 || gamma > 1 || beta < 0 || beta > 1 || beta + gamma > 1)
            {
                t = 0;
                barycentricCoordinate = new Vector3(0, 0, 0);
                return false;
            }
            t = (f * (a * k - j * b) + e * (j * c - a * l) + d * (b * l - k * c)) / M;
            barycentricCoordinate = new Vector3(1 - gamma - beta, beta, gamma);
            return true;
        }

        private Vector2 GetUVCoordinate(Vector2 vertexA, Vector2 vertexB, Vector2 vertexC, Vector3 barycentricCoordinate)
        {
            return vertexA * barycentricCoordinate.x + vertexB * barycentricCoordinate.y + vertexC * barycentricCoordinate.z;
        }
    }
}
