Shader "Terrain" {
  Properties {

    _NormalStrength ("NormalStrength", Float) = 1

    // [Header(Debug)][Space]
    // [KeywordEnum(None, Mesh, Texture, Difference)] _Debug_Normal("Normal", Range(0, 1)) = 0
    // [KeywordEnum(None, Mesh, Texture, Difference)] _Debug_Normal("Normal", Range(0, 1)) = 0
    // _Debug_Normal_Bloving("Blove Normals", Range(0, 1)) = 1

    // [Header(Features)][Space]
    // [KeywordEnum(Normal, Stochastic)] _Tile("Tile Mode", Float) = 0
    // [KeywordEnum(None, Mesh, Texture, Difference)] _Debug_Normal("Normal", Range(0, 1)) = 0
    // _Debug_Normal_Bloving("Blove Normals", Range(0, 1)) = 1

    // [Header(Global)][Space]
    [NoScaleOffset] _Heightmap("Heightmap", 2D) = "white" {}
    // [NoScaleOffset] _Splatmap("Splatmap", 2D) = "black" {}
    // _NormalStrength("Normal Strength", Range(0, 100)) = 1

    // [Header(Layer 0)][Space]
    // [NoScaleOffset] _Layer0Albedo("_Layer0Albedo", 2D) = "black" {}
    // [NoScaleOffset] _Layer0Height("_Layer0Height", 2D) = "black" {}
    // [NoScaleOffset] _Layer0Normal("_Layer0Normal", 2D) = "bump" {}
    // [NoScaleOffset] _Layer0Roughness("_Layer0Roughness", 2D) = "grey" {}
    // [NoScaleOffset] _Layer0AO("_Layer0AO", 2D) = "white" {}

    // [Header(Layer 1)][Space]
    // [NoScaleOffset] _Layer1Albedo("_Layer1Albedo", 2D) = "black" {}
    // [NoScaleOffset] _Layer1Height("_Layer1Height", 2D) = "black" {}
    // [NoScaleOffset] _Layer1Normal("_Layer1Normal", 2D) = "bump" {}
    // [NoScaleOffset] _Layer1Roughness("_Layer1Roughness", 2D) = "grey" {}
    // [NoScaleOffset] _Layer1AO("_Layer1AO", 2D) = "white" {}

    // [Header(Layer 2)][Space]
    // [NoScaleOffset] _Layer2Albedo("_Layer2Albedo", 2D) = "black" {}
    // [NoScaleOffset] _Layer2Height("_Layer2Height", 2D) = "black" {}
    // [NoScaleOffset] _Layer2Normal("_Layer2Normal", 2D) = "bump" {}
    // [NoScaleOffset] _Layer2Roughness("_Layer2Roughness", 2D) = "grey" {}
    // [NoScaleOffset] _Layer2AO("_Layer2AO", 2D) = "white" {}

    // [Header(Layer 3)][Space]
    // [NoScaleOffset] _Layer3Albedo("_Layer3Albedo", 2D) = "black" {}
    // [NoScaleOffset] _Layer3Height("_Layer3Height", 2D) = "black" {}
    // [NoScaleOffset] _Layer3Normal("_Layer3Normal", 2D) = "bump" {}
    // [NoScaleOffset] _Layer3Roughness("_Layer3Roughness", 2D) = "grey" {}
    // [NoScaleOffset] _Layer3AO("_Layer3AO", 2D) = "white" {}

    // [HideInInspector] _texcoord("", 2D) = "white" {}
    // [HideInInspector] _texcoord2("", 2D) = "white" {}
    // [HideInInspector] __dirty("", Int) = 1
  }
  SubShader {
    // Tags {
    //   "RenderType" = "Opaque"
    //   "Queue" = "Geometry"
    // }
    // Cull Back
    Pass {
			// Tags {
			// 	"LightMode" = "Deferred"
			// }
      CGPROGRAM
      #pragma target 5.0

      #include "Uniforms.cginc"
      #include "Scaffold.cginc"
      #include "Noise.cginc"

      // uniform float _FeatureScale;
      // uniform float _HeightScale;
      // uniform float _WorldSize;
      // uniform float _LodDepth;

      uniform sampler2D _Heightmap;
      uniform float4 _Heightmap_TexelSize;

      void vert (inout Vertex v, inout Fragment f) {
        // v.position.xyz += normalize(v.uv) * SampleHeight(normalize(v.uv) * 10, 2, 1.7).w * 100;
      }

      #ifdef UNITY_COMPILER_HLSL
      #define ddx ddx_fine
      #define ddy ddy_fine
      #endif

      // #pragma fragmentoption ARB_precision_hint_nicest

      // float3 unit (float3 n) {
      //   return n * rsqrt(n.x * n.x + n.y * n.y + n.z * n.z);
      // }

      void frag (Fragment f, inout Surface s) {
        // s.occlusion = 1 - (s.albedo = s.specular = 0);
        // float4 n = f.uv;
        // s.albedo = h;
        // s.albedo = normalize(f.normal);
        // f.uv.xyz = f.normal;
        // float3 worldPos = f.normal * _TerrainScale + f.normal * h * _TerrainHeightScale;

        // s.normal = normalize(n.xyz);

        // s.smoothness = lerp(0.4, 0.6, t);

        // s.normal = SampleNormal(f.uv);

        f.color = tex2D(_Heightmap, f.uv);

        float3 origNormal = s.normal;
        float3 dataNormal = -normalize(float3(f.color.gb * _Heightmap_TexelSize.zw, -_HeightScale).xzy);

        s.normal = dataNormal;

        float t = 1 - smoothstep(0.9, 1.0, s.normal.y);
        float h = f.worldPos.y;
        s.albedo = lerp(
          lerp(
            float3(0.3, 0.5, 0.1) / 4,
            float3(0.4, 0.5, 0.1) / 4,
            saturate(h)
          ),
          float3(0.3, 0.3, 0.3) / 2,
          t
        );
        s.smoothness = lerp(0.4, 0.5, t);

        s.albedo = lerp(s.albedo, float3(0, 0, 0.1), f.color.r);
        s.smoothness = lerp(s.smoothness, 0.75, f.color.r);
        
        // s.emission.rgb = tex2D(_Heightmap, f.uv).rgb;
        // s.albedo = 0;
        // s.specular = 0;

        // s.albedo = f.color.a;

        // s.smoothness = 0.5;
        // s.albedo = 0.5 + 0.5 * OctaveNoise(f.uv.xy);
        // f.worldPos.y = SampleHeight(f.uv) * _HeightScale;
        // s.normal = normalize(cross(ddy(f.worldPos), ddx(f.worldPos)));
        // s.normal = SampleNormal(f.uv, 1 / _WorldSize);
        // s.albedo = s.normal;
        // s.albedo.b = _LodDepth;
        // s.albedo.r = smoothstep(0.99, 1, s.normal.y);
        // s.occlusion = 0;
        // s.emission = s.normal * 0.5 + 0.5;
        // s.emission = n * 0.5 + 0.5;
        // s.albedo = n;

        // s.normal = SampleNormal(f.uv);
        // s.normal = float3(0, 1, 0);
        // f.uv = normalize(f.uv);
        // f.uv = f.uv * scale + f.uv * h * heightScale * 0.5;
        // s.emission = SampleNormal(f.uv.xz, 1);
        // s.albedo *= lerp(0.2, 0.9, smoothstep(0.2, 0.8, saturate(h * 0.5 + 0.5)));
      }

      ENDCG
    }
  }
  Fallback "Diffuse"
}
