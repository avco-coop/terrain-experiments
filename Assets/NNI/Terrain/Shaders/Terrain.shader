Shader "Terrain" {
  Properties {
    [KeywordEnum(None, Naive, Sensible)] _Sample("Sample Mode", Float) = 0
  }
  CGINCLUDE

  #include "Uniforms.cginc"
  #include "Types.cginc"

  void frag (Fragment f, inout Surface s) {
    #if defined(_SAMPLE_NAIVE)
      f.color = tex2D(_Heightmap, f.uv);
    #elif defined(_SAMPLE_SENSIBLE)
      f.color = tex2Dgrad(_Heightmap, f.uv, _Heightmap_TexelSize.x * 2, _Heightmap_TexelSize.y * 2);
    #endif
    s.normal = -normalize(float3(f.color.gb * _Heightmap_TexelSize.zw, -_HeightScale).xzy);
    float t = 1 - smoothstep(0.6, 1.0, s.normal.y);
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
    s.albedo = lerp(s.albedo, float3(0, 0, 0), f.color.r);
    s.smoothness = lerp(s.smoothness, 0.75, f.color.r);
  }

  ENDCG
  SubShader {
    Pass {
			Tags {
				"LightMode" = "Deferred"
			}
      CGPROGRAM
      #pragma target 5.0
      #pragma vertex _vert
      #pragma fragment _frag
      #pragma multi_compile _SAMPLE_NONE _SAMPLE_NAIVE _SAMPLE_SENSIBLE
      #pragma multi_compile _ UNITY_HDR_ON
      #define DEFERRED_PASS
			#pragma exclude_renderers nomrt
      #include "Scaffold.cginc"
      ENDCG
    }
    Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}
      CGPROGRAM
      #pragma target 5.0
      #pragma vertex _vert
      #pragma fragment _frag
      #pragma multi_compile _SAMPLE_NONE _SAMPLE_NAIVE _SAMPLE_SENSIBLE
      #pragma multi_compile_fog
      #pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile_fwdadd
      
			#pragma multi_compile _ UNITY_HDR_ON
      #define FORWARD_BASE_PASS
      #include "Scaffold.cginc"
      ENDCG
    }
  }
  Fallback "Diffuse"
}
