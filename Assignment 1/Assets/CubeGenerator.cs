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
        int print = 1219000;
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
             * Summary: 
             * implement ray-triangle intersection and 
             * visualize the barycentric coordinate on each of the triangles of a cube, 
             * with Red, Green and Blue for each coordinate.
             * 
             * Arguments:
             * int width - width of the returned texture
             * int height - height of the return texture
             * 
             * Return:
             *    Texture2D - Texture2D object which contains the rendered result
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
             * Summary:
             * implement UV mapping with the calculated barycentric coordinate in the previous step, 
             * and visualize a texture image on each face of the cube.
             * 
             * Arguments:
             * int width - width of the returned texture
             * int height - height of the return texture
             * Texture2D inputTexture - the texture you need to sample from
             * 
             * Return:
             *   Texture2D - Texture2D object which contains the rendered result
            */
            int imageWidth = inputTexture.width;
            int imageHeight = inputTexture.height;
            CubeResult = new Texture2D(width, height);
            CanvasHeight = height;
            CanvasWidth = width;
            // Texture coordinate
            Vector2 u0 = new Vector2(0, 0);
            Vector2 u1 = new Vector2(1, 0);
            Vector2 v0 = new Vector2(0, 1);
            Vector2 v1 = new Vector2(1, 1);
                
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 RayDirection = Vector3.Normalize(new Vector3((-ViewportWidth / 2) + x * ViewportWidth / CanvasWidth, (-ViewportHeight / 2) + y * ViewportHeight / CanvasHeight, 1));
                    float t;
                    Vector3 barycentricCoordinate;
                    Vector2 uv;
                    if (IntersectTriangle(RayOrigin, RayDirection, VertexA, VertexC, VertexB, out t, out barycentricCoordinate))
                    {
                        uv = GetUVCoordinate(u0, v0, u1, barycentricCoordinate, imageWidth, imageHeight, false, false);
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexC, VertexD, out t, out barycentricCoordinate))
                    {
                        uv = GetUVCoordinate(u1, v0, v1, barycentricCoordinate, imageWidth, imageHeight, false, false);
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexD, VertexE, out t, out barycentricCoordinate))
                    {
                        uv = GetUVCoordinate(u0, v0, v1, barycentricCoordinate, imageWidth, imageHeight, true, false);
                    }
                    else if (IntersectTriangle(RayOrigin, RayDirection, VertexB, VertexE, VertexF, out t, out barycentricCoordinate))
                    {
                        uv = GetUVCoordinate(u0, v1, u1, barycentricCoordinate, imageWidth, imageHeight, true, false);
                    }
                    else
                    {
                        CubeResult.SetPixel(x, y, Color.grey);
                        continue;
                    }
                    CubeResult.SetPixel(x, y, BilinearInterplation(uv, inputTexture));
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
             * Summary:
             * Check whether a given ray hit a triangle or not based on barycentric coordinate
             * 
             * Arguments:
             * Vector3 rayOrigin - origin point of the ray
             * Vector3 rayDirection - the direction of the ray
             * vertexA, vertexB, vertexC - 3 vertices of the target triangle
             * out float t - distance the ray travelled to hit a point
             * out Vector3 barycentricCoordinate - barycentric coordinate value
             * 
             * Return:
             *   bool - indicating hit or not
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

        private Vector2 GetUVCoordinate(Vector2 vertexA, Vector2 vertexB, Vector2 vertexC, Vector3 barycentricCoordinate, int imageWidth, int imageHeight, bool reflectHorz, bool reflectVert)
        {
            /*
             * Summary:
             * Get the UV coordinate based on barycentric coordinate and reflect image vertically or horizontally
             * 
             * Arguments:
             * Vector2 vertexA, vertexB, vertexC form a barycentric coordinate system
             * out Vector3 barycentricCoordinate - barycentric coordinate value
             * int imageWidth - the width of image
             * int imageHeight - the height of image
             * bool reflectHorz - indicate whether relect the image horizontally or not
             * bool reflectVert - indicate whether relect the image vertically or not
             * 
             * Return:
                Vector2 - coordinate value in image plane
            */
            Vector2 uv = vertexA * barycentricCoordinate.x + vertexB * barycentricCoordinate.y + vertexC * barycentricCoordinate.z;
            if (reflectHorz)
            {
                uv.x = imageWidth - uv.x * imageWidth;
            }
            else 
            {
                uv.x = uv.x * imageWidth;
            }

            if (reflectVert)
            {
                uv.y = imageHeight - uv.y * imageHeight;
            }
            else
            {
                uv.y = uv.y * imageHeight;
            }
            return uv;
        }

        private Color BilinearInterplation(Vector2 uv, Texture2D inputTexture)
        {
            /*
             * Summary:
             * Implement the Bilinear Interplation of a given pixel coordinate
             * and return the interplated color
             * 
             * Arguments:
             * Vector2 uv - the coordinate value in image plane
             * exture2D - input image object
             *
             * Return:
             *    Color - the color based on bilinear interplation at uv
            */
            Vector2 A = new Vector2(Mathf.Floor(uv.x), Mathf.Ceil(uv.y));
            Vector2 B = new Vector2(Mathf.Floor(uv.x), Mathf.Floor(uv.y));
            Vector2 C = new Vector2(Mathf.Ceil(uv.x), Mathf.Floor(uv.y));
            Vector2 D = new Vector2(Mathf.Ceil(uv.x), Mathf.Ceil(uv.y));

            float S = (uv.x - A.x) / (D.x - A.x);
            float T = (uv.y - B.y) / (D.y - C.y);

            return (1 - S) * (1 - T) * inputTexture.GetPixel(Convert.ToInt32(B.x), Convert.ToInt32(B.y)) +
                                                          (1 - S) * T * inputTexture.GetPixel(Convert.ToInt32(A.x), Convert.ToInt32(A.y)) +
                                                          S * (1 - T) * inputTexture.GetPixel(Convert.ToInt32(C.x), Convert.ToInt32(C.y)) +
                                                          S * T * inputTexture.GetPixel(Convert.ToInt32(D.x), Convert.ToInt32(D.y));
        }
    }
}
