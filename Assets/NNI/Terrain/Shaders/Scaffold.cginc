#pragma vertex _vert
#pragma fragment _frag

#include "UnityStandardUtils.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#if 1
#define DEFTEXTURE(name) sampler2D name
#define TEXTURE(name, uv) tex2D(name, uv)
#define TEXTUREGRAD(name, uv, dx, dy) tex2Dgrad(name, uv, dx, dy)
#else
#define DEFTEXTURE(name) UNITY_DECLARE_TEX2D_NOSAMPLER(name)
#define TEXTURE(name, uv) UNITY_SAMPLE_TEX2D_SAMPLER(name, _linear_repeat, uv)
#define TEXTUREGRAD(name, uv, dx, dy) UNITY_SAMPLE_TEX2D_SAMPLER(name, _linear_repeat, uv)
#endif

struct Vertex {
  float4 position: POSITION;
  float4 uv: TEXCOORD0;
  float4 normal: NORMAL;
  float4 color: COLOR;
};

struct Fragment {
  float4 position: SV_POSITION;
  float4 uv: TEXCOORD0;
  float4 worldPos: TEXCOORD1;
  float4 localPos: TEXCOORD2;
  float4 origin: TEXCOORD3;
  float3 normal: TEXCOORD4;
  float4 color: COLOR;
};

struct Surface {
  float3 albedo;
  float3 specular;
  float  smoothness;
  float3 normal;
  float  occlusion;
  float3 emission;
  float  height;
};

struct Output {
  float4 gBuffer0 : SV_Target0;
  float4 gBuffer1 : SV_Target1;
  float4 gBuffer2 : SV_Target2;
  float4 gBuffer3 : SV_Target3;
};

UnityLight CreateLight () {
  UnityLight light;
  light.dir = float3(0, 1, 0);
  light.color = 0;
  return light;
}

UnityIndirect CreateIndirectLight (Fragment f, Surface s) {
  UnityIndirect indirectLight;
  indirectLight.specular = 0;
  indirectLight.diffuse = max(0, ShadeSH9(float4(s.normal, 1)));
  indirectLight.diffuse *= s.occlusion;
  return indirectLight;
}

void frag (Fragment f, inout Surface s);

void vert (inout Vertex v, inout Fragment f);

void _vert (Vertex v, out Fragment f) {
  f = (Fragment) 0;
  vert(v, f);
  f.position = UnityObjectToClipPos(v.position);
  f.worldPos = mul(unity_ObjectToWorld, v.position);
  f.localPos = v.position;
  f.origin = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
  f.uv = v.uv;
  f.normal = UnityObjectToWorldNormal(v.normal);
  f.color = v.color;
}

#ifdef DEFERRED

void _frag (Fragment f, out Output o) {
  float3 viewDir = normalize(_WorldSpaceCameraPos - f.worldPos);
  Surface s = (Surface) 0;
  s.albedo = 0.5;
  s.smoothness = 0.5;
  s.specular = 0.04;
  s.occlusion = 1;
  s.normal = f.normal;
  // s.normal = normalize(cross(ddy(f.worldPos), ddx(f.worldPos)));
  frag(f, s);
  float3 color = UNITY_BRDF_PBS(
    s.albedo,
    s.specular,
    0, //oneMinusReflectivity,
    s.smoothness,
    s.normal,
    viewDir,
    CreateLight(),
    CreateIndirectLight(f, s)
  ) + s.emission;
  o.gBuffer0.rgb = s.albedo;
  o.gBuffer0.a = 1 - s.occlusion;
  o.gBuffer1.rgb = s.specular;
  o.gBuffer1.a = s.smoothness;
  o.gBuffer2.rgb = s.normal * 0.5 + 0.5;
  o.gBuffer2.a = 1;
  o.gBuffer3.rgb = color;
  o.gBuffer3.a = 1;
}

#else

#include "UnityStandardCore.cginc"

void _frag (Fragment f, out float4 o : SV_TARGET) {
  float3 viewDir = normalize(_WorldSpaceCameraPos - f.worldPos);
  Surface s = (Surface) 0;
  s.albedo = 0.5;
  s.smoothness = 0.5;
  s.specular = 0.04;
  s.occlusion = 0;
  s.normal = f.normal;
  // s.normal = normalize(cross(ddy(f.worldPos), ddx(f.worldPos)));
  frag(f, s);
  float3 color = UNITY_BRDF_PBS(
    s.albedo,
    s.specular,
    0, //oneMinusReflectivity,
    s.smoothness,
    s.normal,
    viewDir,
    MainLight(),
    ZeroIndirect()
  ) + s.emission;
  o.rgb = color;
  o.a = 1;
}

#endif
