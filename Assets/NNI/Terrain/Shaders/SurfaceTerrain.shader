Shader "Surface Terrain"
{
  Properties
  {
    _EdgeLength ("Edge length", Range(2,50)) = 15
    _NormalScale ("Normal Scale", Float) = 1
    // [KeywordEnum(None, Naive, Sensible)] _Sample("Sample Mode", Float) = 0
    [KeywordEnum(Mesh, Shader)] _Normals("Normal Source", Float) = 0
  }
  SubShader
  {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
    #pragma surface surf Standard addshadow fullforwardshadows tessellate:tessEdge
    #pragma target 3.0
    #pragma multi_compile _SAMPLE_NONE _SAMPLE_NAIVE _SAMPLE_SENSIBLE
    #pragma multi_compile _NORMALS_MESH _NORMALS_SHADER

    #include "Tessellation.cginc"
    #include "Noise.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    sampler2D _Heightmap;
    float4 _Heightmap_TexelSize;

    float _HeightScale, _WorldScale, _PatchSize, _Resolution;

    float _NormalScale;

    float _EdgeLength;

    struct Input {
      float3 color : Color;
      float2 uv_MainTex;
      float2 uv_Heightmap;
      float3 worldPos;
      float3 worldNormal;
      INTERNAL_DATA
    };

    UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_INSTANCING_BUFFER_END(Props)

    float4 tessEdge (appdata_full v0, appdata_full v1, appdata_full v2) {
      return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
    }

    void disp (inout appdata_full v) {
      // v.texcoord.xy *= 0.5;
      // v.texcoord.xy += 0.5;
      // v.texcoord.xy *= _Heightmap_TexelSize.zw / (_Heightmap_TexelSize.zw + 1.5);
      // float d = tex2Dlod(_Heightmap, float4(v.texcoord.xy,0,0)).r * _HeightScale / _WorldScale;
      // v.vertex.y = d;
    }

    float3 sampleGradient (float2 uv) {
      float2 s = _MainTex_TexelSize.xy;
      float3 f = float3(-1, 0, +1);
      float c = tex2D(_Heightmap, uv + s * f.yy).r;
      float l = tex2D(_Heightmap, uv + s * f.xy).r;
      float r = tex2D(_Heightmap, uv + s * f.zy).r;
      float b = tex2D(_Heightmap, uv + s * f.yx).r;
      float t = tex2D(_Heightmap, uv + s * f.yz).r;
      return float3(l - r, b - t, c);
    }

    float3 gradientToNormal (float2 g) {
      return normalize(float3(
        g.xy * _MainTex_TexelSize.zw,
        1
      ));
      // return normalize(float3(
      //   g.x * _HeightScale * _Resolution * _PatchSize,
      //   g.y * _HeightScale * _Resolution * _PatchSize,
      //   1
      // ));
    }

    void surf (Input IN, inout SurfaceOutputStandard o) {
      float3 g = sampleGradient(IN.uv_MainTex);
      // IN.uv_MainTex = 1 - IN.uv_MainTex;
      // float4 samp = tex2D(_Heightmap, IN.uv_MainTex);
      // float3 normal = -normalize(float3(samp.gb * _MainTex_TexelSize.zw, -_HeightScale).xyz);
      float3 normal = gradientToNormal(g);
      // normal = normalize(g * float3(_MainTex_TexelSize.wz, 1));
      o.Albedo = 0.5;
      #if defined(_NORMALS_MESH)
        normal = IN.worldNormal.xzy;
      #else
        o.Normal = normalize(normal);
      #endif
      o.Smoothness = 0.1;

      // o.Metallic = 1;
      // o.Albedo = 0;
      // o.Occlusion = 0;
      // o.Emission = normalize(g * float3(_MainTex_TexelSize.xy, 0)).xzy * 0.5 + 0.5;

      float t = 1 - smoothstep(0.6, 1.0, normal.z);
      float h = g.z;
      o.Albedo = lerp(
        lerp(
          normalize(float3(0.1, 0.3, 0.0)) / 8,
          normalize(float3(0.6, 0.4, 0.0)) / 8,
          saturate(h * 50)
        ),
        lerp(
          0.5,
          lerp(
            float3(0.3, 0.1, 0.0),
            float3(0.5, 0.2, 0.1),
            sin(h + OctaveNoise(h * 100, 2))
          ),
          0.75
        ),
        t
      );
      o.Smoothness = lerp(0.2, 0.5, t);

      o.Albedo = lerp(o.Albedo, float3(0.3, 0.2, 0.1) / 4, smoothstep(0, -0.0012, h));
      o.Smoothness = lerp(o.Smoothness, 0.1, smoothstep(0, -0.001, h));

      // o.Albedo = lerp(o.Albedo, float3(0, 0, 0), IN.color.r);
      // o.Smoothness = lerp(o.Smoothness, 0.75, IN.color.r);
      // #endif
    }
    ENDCG
  }
  FallBack "Diffuse"
}
