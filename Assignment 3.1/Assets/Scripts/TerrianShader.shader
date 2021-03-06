﻿Shader "Custom/Terrain" {
	Properties{
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			const static int numLayers = 4;
			const static float epsilon = 1E-4;
            const static float minHeight = 50;
            const static float maxHeight = 100;

			float sunIntensity;
			float sunPosX;
			float sunPosY;
			float sunPosZ;
			float3 baseTints[numLayers];
			float baseStartLevels[numLayers];
			float baseBlends[numLayers];
			float baseTextureScales[numLayers];
            float baseColorStrength[numLayers];

			UNITY_DECLARE_TEX2DARRAY(baseTextures);

			struct Input {
				float3 worldPos;
				float3 worldNormal;
			};

			float inverseLinearInterp(float a, float b, float v) {
				return saturate((v - a) / (b - a));
			}

			float3 calculateTextureColor(float3 worldPos, float textureScale, float3 blendAxes, int textureIndex) {
				float3 scaledWorldPos = worldPos / textureScale;
				float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
				float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
				float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
				return xProjection + yProjection + zProjection;
			}

			float3 normalVector(float3 v) {
				return (v) / (pow(v.x * v.x + v.y * v.y + v.z * v.z, 0.5));
			}

			float3 dotProduct(float3 x, float3 y) {
				return x.x * y.x + x.y * y.y + x.z * y.z;
			}

			float phongBlinn(Input IN, float3 viewDir) {
				float3 sunPos = float3(sunPosX, sunPosY, sunPosZ);
				float H = float3(sunPos.x + viewDir.x, sunPos.y + viewDir.y, sunPos.z + viewDir.z);
				return dotProduct(normalVector(H), normalVector(IN.worldNormal));
			}

			void surf(Input IN, inout SurfaceOutputStandard o) {
				float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
				float heightLevel = inverseLinearInterp(minHeight, maxHeight, IN.worldPos.y);
				float3 blendAxes = abs(IN.worldNormal);
				blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
				for (int i = 0; i < numLayers; i++) {
					float drawStrength = inverseLinearInterp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightLevel - baseStartLevels[i]);
					float3 textureColor = calculateTextureColor(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColorStrength[i]);
					o.Albedo = o.Albedo * (1 - drawStrength) + (baseTints[i] + textureColor) * drawStrength;
					o.Albedo += sunIntensity * 0.1 * pow(phongBlinn(IN, viewDir), 1);
				}
			}
			ENDCG
		}
			FallBack "Diffuse"
}