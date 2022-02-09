#ifndef TYPES_INCLUDED
#define TYPES_INCLUDED

#include "AutoLight.cginc"

struct Vertex {
  float4 position: POSITION;
  float2 uv: TEXCOORD0;
  float4 normal: NORMAL;
  float4 color: COLOR;
};

struct Fragment {
  float4 position: SV_POSITION;
  float2 uv: TEXCOORD0;
  float3 worldPos: TEXCOORD1;
  float4 localPos: TEXCOORD2;
  float4 origin: TEXCOORD3;
  float3 normal: TEXCOORD4;
  float4 color: COLOR;
	SHADOW_COORDS(5)
};

struct Surface {
  float3 albedo;
  float  metallic;
  float  smoothness;
  float3 normal;
  float  occlusion;
  float3 emission;
  float  height;
};

#endif
