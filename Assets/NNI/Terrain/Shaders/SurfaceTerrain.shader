Shader "Surface Terrain"
{
  Properties
  {
    [KeywordEnum(None, Naive, Sensible)] _Sample("Sample Mode", Float) = 0
    [KeywordEnum(None, Basic)] _Color("Color Mode", Float) = 0
  }
  SubShader
  {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows
    #pragma target 3.0
    #pragma multi_compile _SAMPLE_NONE _SAMPLE_NAIVE _SAMPLE_SENSIBLE
    #pragma multi_compile _COLOR_NONE _COLOR_BASIC

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    struct Input
    {
      float2 uv_MainTex;
      float3 color : Color;
      float3 worldPos;
    };

    float _HeightScale;
    half _Glossiness;
    half _Metallic;
    fixed4 _Color;

    UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_INSTANCING_BUFFER_END(Props)

    void surf (Input IN, inout SurfaceOutputStandard o)
    {
      #if defined(_SAMPLE_NAIVE)
        IN.color = tex2D(_MainTex, IN.uv_MainTex);
      #elif defined(_SAMPLE_SENSIBLE)
        IN.color = tex2Dgrad(_MainTex, IN.uv_MainTex, _MainTex_TexelSize.x * 2, _MainTex_TexelSize.y * 2);
      #endif
      o.Normal = -normalize(float3(IN.color.gb * _MainTex_TexelSize.zw, -_HeightScale).xyz);
      #if defined(_COLOR_NONE)
      o.Albedo = 0.5;
      o.Smoothness = 0.7;
      #elif defined(_COLOR_BASIC)
      float t = 1 - smoothstep(0.6, 1.0, o.Normal.z);
      float h = IN.worldPos.y;
      o.Albedo = lerp(
        lerp(
          float3(0.3, 0.5, 0.1) / 4,
          float3(0.4, 0.5, 0.1) / 4,
          saturate(h)
        ),
        float3(0.3, 0.3, 0.3) / 2,
        t
      );
      o.Smoothness = lerp(0.4, 0.5, t);
      o.Albedo = lerp(o.Albedo, float3(0, 0, 0), IN.color.r);
      o.Smoothness = lerp(o.Smoothness, 0.75, IN.color.r);
      #endif
    }
    ENDCG
  }
  FallBack "Diffuse"
}
