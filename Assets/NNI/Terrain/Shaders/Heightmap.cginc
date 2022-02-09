
float SampleHeight(float2 uv) {
  return tex2D(_Heightmap, uv) * _HeightScale;
}

void CalculateUVsSmooth (float2 UV, float2 TexelSize, out float2 UV0, out float2 UV1, out float2 UV2, out float2 UV3, out float2 UV4, out float2 UV5, out float2 UV6, out float2 UV7, out float2 UV8) {
  float3 pos = float3(TexelSize.xy, 0);
  float3 neg = float3(-pos.xy, 0);
  UV0 = UV + neg.xy;
  UV1 = UV + neg.zy;
  UV2 = UV + float2(pos.x, neg.y);
  UV3 = UV + neg.xz;
  UV4 = UV;
  UV5 = UV + pos.xz;
  UV6 = UV + float2(neg.x, pos.y);
  UV7 = UV + pos.zy;
  UV8 = UV + pos.xy;
  return;
}

float4 CombineSamplesSmooth (float Strength, float S0, float S1, float S2, float S3, float S4, float S5, float S6, float S7, float S8) {
  float4 normal;
  normal.x = Strength * (S0 - S2 + 2 * S3 - 2 * S5 + S6 - S8);
  normal.y = 1;
  normal.z = Strength * (S0 + 2 * S1 + S2 - S6 - 2 * S7 - S8);
  normal.xyz = normalize(normal.xyz);
  normal.w = S4;
  return normal;
}

float4 SampleNormal(float2 uv, float2 texelSize, float strength) {
  float2 uv0, uv1, uv2, uv3, uv4, uv5, uv6, uv7, uv8;
  CalculateUVsSmooth(
    uv, texelSize,
    uv0, uv1, uv2, uv3, uv4, uv5, uv6, uv7, uv8
  );
  return CombineSamplesSmooth(
    strength,
    SampleHeight(uv0).r,
    SampleHeight(uv1).r,
    SampleHeight(uv2).r,
    SampleHeight(uv3).r,
    SampleHeight(uv4).r,
    SampleHeight(uv5).r,
    SampleHeight(uv6).r,
    SampleHeight(uv7).r,
    SampleHeight(uv8).r
  );
}

float4 SampleNormal(float2 uv) {
  return SampleNormal(uv, _Heightmap_TexelSize.xy, _HeightScale / _WorldSize);
}
