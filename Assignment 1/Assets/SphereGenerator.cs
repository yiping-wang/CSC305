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
    public class SphereGenerator
    {
        Vector3 SphereCeneter;
        Vector3 LightLocation;
        Texture2D SphereResult;
        Vector3 ViewLocation;
        Vector3 RayOrigin;
        float LightIntensity;
        float SphereRadius;
        float ViewportWidth;
        float ViewportHeight;
        int CanvasWidth;
        int CanvasHeight;

        public SphereGenerator()
        {
            SphereCeneter = new Vector3(0, 0, 10);
            RayOrigin = new Vector3(0, 0, 0);
            ViewLocation = new Vector3(0, 0, 0);
            SphereRadius = 5;
            LightLocation = new Vector3(7 * SphereRadius, 7 * SphereRadius, -5);
            ViewportWidth = 4;
            LightIntensity = 4f;
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
            CanvasHeight = height;
            CanvasWidth = width;
            ViewportHeight = (float)CanvasHeight / (float)CanvasWidth * ViewportWidth;
            SphereResult = new Texture2D(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // Perspective projection
                    Vector3 RayDirection = Vector3.Normalize(new Vector3((-ViewportWidth / 2) + x * ViewportWidth / CanvasWidth, (-ViewportHeight / 2) + y * ViewportHeight / CanvasHeight, 1));
                    float t;
                    Vector3 intersectNormal;
                    if (IntersectSphere(RayOrigin, RayDirection, SphereCeneter, SphereRadius, out t, out intersectNormal))
                    {
                        Vector3 surfacePoint = RayOrigin + t * RayDirection;
                        float color = Lambertian(0.2f, intersectNormal, surfacePoint, LightLocation) + BlinnPhong(0.2f, intersectNormal, surfacePoint, ViewLocation, LightLocation, 10f) + Ambient(0.02f);
                        SphereResult.SetPixel(x, y, new Color(0.7f * color, 0.7f * color, 0));
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
        private bool IntersectSphere(Vector3 rayOrigin,
                                        Vector3 rayDirection,
                                        Vector3 sphereCenter,
                                        float sphereRadius,
                                        out float t,
                                        out Vector3 intersectNormal
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
            float A = Vector3.Dot(rayDirection, rayDirection);
            float B = 2 * Vector3.Dot(rayDirection, (rayOrigin - sphereCenter));
            float C = Vector3.Dot(rayOrigin - sphereCenter, rayOrigin - sphereCenter) - sphereRadius * sphereRadius;
            float D = B * B - 4 * A * C;

            if (D < 0)
            {
                t = 0;
                intersectNormal = new Vector3(0, 0, 0);
                return false;
            }

            float t0 = (-B + Mathf.Sqrt(D)) / (2 * A);
            float t1 = (-B - Mathf.Sqrt(D)) / (2 * A);

            t = Mathf.Min(t0, t1);
            intersectNormal = Vector3.Normalize(rayOrigin + t * rayDirection - sphereCenter);
            return true;
        }

        private float Lambertian(float diffuseCoefficient, Vector3 intersectNormal, Vector3 surfacePoint, Vector3 lightLocation)
        {
            Vector3 lightDirection = Vector3.Normalize(lightLocation - surfacePoint);
            return diffuseCoefficient * LightIntensity * Mathf.Max(0, Vector3.Dot(intersectNormal, lightDirection));
        }

        private float BlinnPhong(float specularCoefficient, Vector3 intersectNormal, Vector3 surfacePoint, Vector3 viewLocation, Vector3 lightLocation, float phongExponent)
        {
            Vector3 viewDirection = Vector3.Normalize(viewLocation - surfacePoint);
            Vector3 lightDirection = Vector3.Normalize(lightLocation - surfacePoint);
            Vector3 bisectionDirection = Vector3.Normalize(viewDirection + lightDirection);
            return specularCoefficient * LightIntensity * Mathf.Pow(Mathf.Max(0, Vector3.Dot(intersectNormal, bisectionDirection)), phongExponent);
        }

        private float Ambient(float AmbientCoefficient)
        {
            return AmbientCoefficient * LightIntensity;
        }
    }
}
