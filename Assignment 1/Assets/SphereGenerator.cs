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
    public class SphereGenerator
    {
        Vector3 SphereCeneter;
        Vector3 LightLocation;
        Vector3 ViewLocation;
        float LIGHTINTENSITY;
        float RADIUS;
        int canvasWidth;
        int canvasHeight;

        public SphereGenerator()
        {
            RADIUS = 100;
            LIGHTINTENSITY = 2.5f;
        }

        public Texture2D GenSphere(int width, int height)
        {
            /*
            implement ray-sphere intersection and render a sphere with ambient, diffuse and specular lighting.

            int width - width of the returned texture
            int height - height of the return texture
            return:
                Texture2D - Texture2D object which contains the rendered result
            */
            Texture2D SphereResult = new Texture2D(width, height);
            SphereCeneter = new Vector3(width / 2, height / 2, 200);
            LightLocation = new Vector3(width , height * 2, 800);
            ViewLocation = new Vector3(width, height, 0); // where should I define view location
            canvasHeight = height;
            canvasWidth = width;
            // ray trace from each pixel of camera
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 RayOrigin = new Vector3(x, y, 0);
                    Vector3 RayDirection = new Vector3(0, 0, 1);
                    float t;
                    Vector3 IntersectNormal;
                    if (IntersectSphere(RayOrigin, RayDirection, SphereCeneter, RADIUS, out t, out IntersectNormal))
                    {
                        Vector3 SurfacePoint = RayOrigin + t * RayDirection;
                        float LightColor = Lambertian(0.2f, IntersectNormal, SurfacePoint) + BlinnPhong(0.2f, IntersectNormal, SurfacePoint, 1.5f) + Ambient(0.02f);
                        Color color = new Color(LightColor, 0, 0);
                        SphereResult.SetPixel(x, y, color);
                    }
                    else
                    {
                        SphereResult.SetPixel(x, y, Color.gray);
                    }
                }
            }

            SphereResult.Apply();

            return SphereResult;
        }
        private bool IntersectSphere(Vector3 RayOrigin,
                                        Vector3 RayDirection,
                                        Vector3 SphereCenter,
                                        float SphereRadius,
                                        out float t,
                                        out Vector3 IntersectNormal
                                        )
        {
            /*
            Vector3 origin - origin point of the ray
            Vector3 direction - the direction of the ray
            Vector3 sphereCenter - center of target sphere
            float sphereRadius - radius of target sphere
            out float t - distance the ray travelled to hit a point
            out Vector3 intersectNormal - normal of the hit point
            return:
                bool - indicating hit or not
            */
            float A = Vector3.Dot(RayDirection, RayDirection);
            float B = 2 * Vector3.Dot(RayDirection, (RayOrigin - SphereCenter));
            float C = Vector3.Dot(RayOrigin - SphereCenter, RayOrigin - SphereCenter) - SphereRadius * SphereRadius;
            float D = B * B - 4 * A * C;

            if (D < 0)
            {
                t = 0;
                IntersectNormal = new Vector3(0, 0, 0);
                return false;
            }

            t = (-B + Mathf.Sqrt(D))/(2*A);
            IntersectNormal = Vector3.Normalize(RayOrigin + t * RayDirection - SphereCenter);
            return true;
        }

        private float Lambertian(float DiffuseCoefficient, Vector3 IntersectNormal, Vector3 SurfacePoint)
        {
            Vector3 l = Vector3.Normalize(LightLocation - SurfacePoint);
            return DiffuseCoefficient * LIGHTINTENSITY * Mathf.Max(0, Vector3.Dot(IntersectNormal, l));
        }

        private float BlinnPhong(float SpecularCoefficient, Vector3 IntersectNormal, Vector3 SurfacePoint, float p)
        {
            Vector3 v = Vector3.Normalize(ViewLocation - SurfacePoint);
            Vector3 l = Vector3.Normalize(LightLocation - SurfacePoint);
            Vector3 h = Vector3.Normalize(v + l);
            return SpecularCoefficient * LIGHTINTENSITY * Mathf.Pow(Mathf.Max(0, Vector3.Dot(IntersectNormal, h)), p);
        }

        private float Ambient(float AmbientCoefficient)
        {
            return AmbientCoefficient * LIGHTINTENSITY;
        }
    }
}
