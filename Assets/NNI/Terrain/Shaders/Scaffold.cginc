#ifndef SCAFFOLD_INCLUDED
#define SCAFFOLD_INCLUDED

#include "UnityStandardCore.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"
#include "UnityCG.cginc"

#if 1
#define DEFTEXTURE(name) sampler2D name
#define TEXTURE(name, uv) tex2D(name, uv)
#define TEXTUREGRAD(name, uv, dx, dy) tex2Dgrad(name, uv, dx, dy)
#else
#define DEFTEXTURE(name) UNITY_DECLARE_TEX2D_NOSAMPLER(name)
#define TEXTURE(name, uv) UNITY_SAMPLE_TEX2D_SAMPLER(name, _linear_repeat, uv)
#define TEXTUREGRAD(name, uv, dx, dy) UNITY_SAMPLE_TEX2D_SAMPLER(name, _linear_repeat, uv)
#endif

struct Output {
	#if defined(DEFERRED_PASS)
		float4 gBuffer0 : SV_Target0;
		float4 gBuffer1 : SV_Target1;
		float4 gBuffer2 : SV_Target2;
		float4 gBuffer3 : SV_Target3;
	#else
		float4 color : SV_Target;
	#endif
};

UnityLight CreateLight (Fragment i) {
  UnityLight light;
  #if defined(DEFERRED_PASS)
    light.dir = float3(0, 1, 0);
    light.color = 0;
  #else
    #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
      light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
    #else
      light.dir = _WorldSpaceLightPos0.xyz;
    #endif
		UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
  	light.color = _LightColor0.rgb * attenuation;
  #endif
	light.ndotl = DotClamped(i.normal, light.dir);
	return light;
}

UnityIndirect CreateIndirectLight (Fragment f, Surface s, float3 viewDir) {
  UnityIndirect indirectLight;
  indirectLight.diffuse = 0;
  indirectLight.specular = 0;
	#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
    indirectLight.diffuse += max(0, ShadeSH9(float4(s.normal, 1)));
    float3 reflectionDir = reflect(-viewDir, s.normal);
		Unity_GlossyEnvironmentData envData;
		envData.roughness = 1 - s.smoothness;
		envData.reflUVW = reflectionDir;
		indirectLight.specular += Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData);
    indirectLight.diffuse *= s.occlusion;
		indirectLight.specular *= s.occlusion;
  #endif
  return indirectLight;
}

void frag (Fragment f, inout Surface s);

void _vert (Vertex v, out Fragment f) {
  f = (Fragment) 0;
  f.position = UnityObjectToClipPos(v.position);
  f.worldPos = mul(unity_ObjectToWorld, v.position);
  f.localPos = v.position;
  f.origin = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
  f.uv = v.uv;
  f.normal = UnityObjectToWorldNormal(v.normal);
  f.color = v.color;
#define pos position
  TRANSFER_SHADOW(f);
#undef pos
}

float4 ApplyFog (float4 color, Fragment i) {
	float viewDistance = length(_WorldSpaceCameraPos - i.worldPos);
	UNITY_CALC_FOG_FACTOR_RAW(viewDistance);
  float3 scattering = float3(0.3, 0.2, 0.1) / 10;
  color.rgb *= exp(-scattering * viewDistance);
	color.rgb = lerp(unity_FogColor.rgb, color.rgb, saturate(unityFogFactor));
  return color;
}

void _frag (Fragment f, out Output o) {
  float3 viewDir = normalize(_WorldSpaceCameraPos - f.worldPos);
  Surface s = (Surface) 0;
  s.albedo = 0.5;
  s.smoothness = 0.5;
  s.metallic = 0;
  s.occlusion = 1;
  s.normal = f.normal;
  frag(f, s);
  float3 specular;
  float oneMinusReflectivity;
  s.albedo = DiffuseAndSpecularFromMetallic(
    s.albedo, s.metallic, specular, oneMinusReflectivity
  );
  UnityIndirect il = CreateIndirectLight(f, s, viewDir);
  float3 color = UNITY_BRDF_PBS(
    s.albedo,
    specular,
    oneMinusReflectivity,
    s.smoothness,
    s.normal,
    viewDir,
    CreateLight(f),
    il
  ) + s.emission;
  #ifdef DEFERRED_PASS
		#if !defined(UNITY_HDR_ON)
			color.rgb = exp2(-color.rgb);
		#endif
    o.gBuffer0.rgb = s.albedo;
    o.gBuffer0.a = 1 - s.occlusion;
    o.gBuffer1.rgb = specular;
    o.gBuffer1.a = s.smoothness;
    o.gBuffer2.rgb = s.normal * 0.5 + 0.5;
    o.gBuffer2.a = 1;
    o.gBuffer3.rgb = color;
    o.gBuffer3.a = 1;
  #else
    o.color = ApplyFog(float4(color, 1), f);
  #endif
}

#endif
