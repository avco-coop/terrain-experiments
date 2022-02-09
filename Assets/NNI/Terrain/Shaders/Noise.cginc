#include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise3D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

float SelectedNoise (float3 p) {
  return SimplexNoise(p);
}

float OctaveNoise (float2 p, int n) {
  float g = 0.5;
  float f = 1;
  float a = 1;
  float t = 0;
  float w = 0;
  for (int i = 0; i < n; ++i) {
    p = p.yx + sqrt(2);
    t += a * SelectedNoise(float3(f * p, i + _Seed));
    w += a;
    f *= 1.99;
    a *= g;
  }
  return clamp(t / w, -1, 1);
}

float OctaveNoise (float2 p) {
  return OctaveNoise(p, _Octaves);
}

float2 OctaveNoise2 (float2 p) {
  return float2(
    OctaveNoise(p + float2(1.0, 3.0)),
    OctaveNoise(p + float2(2.0, 4.0))
  );
}

float SampleHeight (float2 p) {
  p *= _Scale;
  float f = OctaveNoise(p / 2 + float2(1.7, 5.2), 3);
  f = saturate(0.2 + 0.8 * f);
  return pow(abs(OctaveNoise(p + f * OctaveNoise2(p))), _HeightExponent) * _HeightScale;
  // return pow(OctaveNoise(p / _Scale), 2) * 5;
}

float4 SampleNormal (float2 p) {
  float3 o = float3(-1, 0, 1) / _WorldSize;
  float s11 = SampleHeight(p);
  float s01 = SampleHeight(p + o.xy) * _HeightScale;
  float s21 = SampleHeight(p + o.zy) * _HeightScale;
  float s10 = SampleHeight(p + o.yx) * _HeightScale;
  float s12 = SampleHeight(p + o.yz) * _HeightScale;
  return float4(normalize(float3(s01 - s21, 2, s10 - s12)), s11);
}

// void CalculateUVsSmooth (float2 UV, float2 TexelSize, out float2 UV0, out float2 UV1, out float2 UV2, out float2 UV3, out float2 UV4, out float2 UV5, out float2 UV6, out float2 UV7, out float2 UV8) {
//   float3 pos = float3(TexelSize.xy, 0);
//   float3 neg = float3(-pos.xy, 0);
//   UV0 = UV + neg.xy;
//   UV1 = UV + neg.zy;
//   UV2 = UV + float2(pos.x, neg.y);
//   UV3 = UV + neg.xz;
//   UV4 = UV;
//   UV5 = UV + pos.xz;
//   UV6 = UV + float2(neg.x, pos.y);
//   UV7 = UV + pos.zy;
//   UV8 = UV + pos.xy;
//   return;
// }

// float4 CombineSamplesSmooth (float Strength, float S0, float S1, float S2, float S3, float S4, float S5, float S6, float S7, float S8) {
//   float4 normal;
//   normal.x = Strength * (S0 - S2 + 2 * S3 - 2 * S5 + S6 - S8);
//   normal.y = 1;
//   normal.z = Strength * (S0 + 2 * S1 + S2 - S6 - 2 * S7 - S8);
//   normal.xyz = normalize(normal.xyz);
//   normal.w = S4;
//   return normal;
// }

// float4 SampleNormal(float2 uv, float2 s) {
//   float2 uv0, uv1, uv2, uv3, uv4, uv5, uv6, uv7, uv8;
//   CalculateUVsSmooth(
//     uv, s,
//     uv0, uv1, uv2, uv3, uv4, uv5, uv6, uv7, uv8
//   );
//   return CombineSamplesSmooth(
//     _HeightScale,
//     SampleHeight(uv0).r,
//     SampleHeight(uv1).r,
//     SampleHeight(uv2).r,
//     SampleHeight(uv3).r,
//     SampleHeight(uv4).r,
//     SampleHeight(uv5).r,
//     SampleHeight(uv6).r,
//     SampleHeight(uv7).r,
//     SampleHeight(uv8).r
//   );
// }
