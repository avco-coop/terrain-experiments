Shader "Generate" {
  Properties {
    _HeightExponent ("Height Exponent", Float) = 1
    _HeightScale ("Height Scale", Float) = 0
    _SeaLevel ("Sea Level", Float) = 0
    _WorldSize ("World Size", Float) = 0
    _Seed ("Seed", Float) = 0
    _Scale ("Scale", Float) = 0
    _Octaves ("Octaves", Float) = 1
    _Offset ("Offset", Vector) = (0, 0, 0, 0)
  }
  SubShader {
    Pass {
      CGPROGRAM
      #pragma target 5.0
      #pragma vertex vert
      #pragma fragment frag

      #include "Uniforms.cginc"
      #include "Noise.cginc"

      struct Vertex {
        float4 position: POSITION;
        float2 uv: TEXCOORD0;
      };

      struct Fragment {
        float4 position: SV_POSITION;
        float2 uv: TEXCOORD0;
      };

      Fragment vert (Vertex v) {
        Fragment o;
        o.position = UnityObjectToClipPos(v.position);
        o.uv = v.uv;
        return o;
      }
 
      float4 frag (Fragment f) : SV_TARGET {
        float4 o = 0;
        float h = SampleHeight(f.uv - 0.5 + _Offset);
        if (h < _SeaLevel)
          o.r = 1;
        else
          o.r = 0;
        h = max(_SeaLevel, h);
        o.g = ddx(h);
        o.b = ddy(h);
        o.w = h;
        return o;
      }

      // void vert (inout Vertex v, inout Fragment f) {
      //   // v.position.xyz += normalize(v.uv) * SampleHeight(normalize(v.uv) * 10, 2, 1.7).w * 100;
      // }

      // #pragma fragmentoption ARB_precision_hint_nicest

      // float3 unit (float3 n) {
      //   return n * rsqrt(n.x * n.x + n.y * n.y + n.z * n.z);
      // }

      // void frag (Fragment f, inout Surface s) {
      //   // s.occlusion = 1 - (s.albedo = s.specular = 0);
      //   // float4 n = f.uv;
      //   // s.albedo = h;
      //   // s.albedo = normalize(f.normal);
      //   // f.uv.xyz = f.normal;
      //   // float3 worldPos = f.normal * _TerrainScale + f.normal * h * _TerrainHeightScale;

      //   // s.normal = normalize(n.xyz);

      //   // s.smoothness = lerp(0.4, 0.6, t);

      //   // s.normal = SampleNormal(f.uv);

      //   float t = 1 - smoothstep(0.9, 1.0, s.normal.y);
      //   float h = f.worldPos.y;
      //   s.albedo = lerp(
      //     lerp(
      //       float3(0.3, 0.5, 0.1) / 2,
      //       float3(0.3, 0.5, 0.1) / 4,
      //       saturate(h)
      //     ),
      //     float3(0.3, 0.3, 0.3) / 2,
      //     t
      //   );
      //   s.smoothness = lerp(0.4, 0.5, t);

      //   // s.smoothness = 0.5;
      //   // s.albedo = 0.5 + 0.5 * OctaveNoise(f.uv.xy);
      //   // f.worldPos.y = SampleHeight(f.uv) * _HeightScale;
      //   // s.normal = normalize(cross(ddy(f.worldPos), ddx(f.worldPos)));
      //   // s.normal = SampleNormal(f.uv, 1 / _WorldSize);
      //   // s.albedo = s.normal;
      //   // s.albedo.b = _LodDepth;
      //   // s.albedo.r = smoothstep(0.99, 1, s.normal.y);
      //   // s.occlusion = 0;
      //   // s.emission = s.normal * 0.5 + 0.5;
      //   // s.emission = n * 0.5 + 0.5;
      //   // s.albedo = n;

      //   // s.normal = SampleNormal(f.uv);
      //   // s.normal = float3(0, 1, 0);
      //   // f.uv = normalize(f.uv);
      //   // f.uv = f.uv * scale + f.uv * h * heightScale * 0.5;
      //   // s.emission = SampleNormal(f.uv.xz, 1);
      //   // s.albedo *= lerp(0.2, 0.9, smoothstep(0.2, 0.8, saturate(h * 0.5 + 0.5)));
      // }

      ENDCG
    }
  }
  Fallback "Diffuse"
}
