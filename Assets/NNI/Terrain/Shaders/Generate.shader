Shader "Generate" {
  Properties {
    _Fudge ("Fudge", Float) = 0
    _HeightExponent ("Height Exponent", Float) = 1
    // _HeightScale ("Height Scale", Float) = 0
    // _SeaLevel ("Sea Level", Float) = 0
    // _WorldSize ("World Size", Float) = 0
    _Seed ("Seed", Float) = 0
    // _Scale ("Scale", Float) = 0

    [Header(Caldera)][Space]
    _CalderaSize ("Size", Float) = 1
    _CalderaSmoothing ("Smoothing", Float) = 1
    _CalderaStrength ("Strength", Float) = 1

    [Header(Island)][Space]
    _IslandOctaves ("Octaves", Float) = 1
    _IslandScale ("Scale", Float) = 1
    _IslandStrength ("Strength", Float) = 1
    // _Offset ("Offset", Vector) = (0, 0, 0, 0)
  }
  SubShader {
    Pass {
      CGPROGRAM
      #pragma target 5.0
      #pragma vertex vert
      #pragma fragment frag

      #include "Noise.cginc"

      float3 _WorldPosition, _LocalPosition;

      float _Resolution;
      float _WorldScale, _HeightScale, _PatchSize;
      float2 _Offset;

      float _Fudge;

      float _HeightExponent;

      float _CalderaSize;
      float _CalderaSmoothing;
      float _CalderaStrength;

      float _IslandOctaves, _IslandStrength, _IslandScale;

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
        o.uv = float2(v.uv.x, v.uv.y);
        o.uv -= 1 / (_Resolution * 2);
        o.uv *= _Resolution / (_Resolution - 1);
        o.uv -= 0.5;
        o.uv += _WorldPosition.xz / _PatchSize;
        o.uv *= _WorldScale;
        return o;
      }
 
      float SampleHeight1 (float2 p) {
        float f = OctaveNoise(p / 2 + float2(1.7, 5.2), 3);
        f = saturate(0.2 + 0.8 * f);
        return pow(abs(OctaveNoise(p + f * OctaveNoise2(p, 10), 10)), _HeightExponent);
        // return pow(OctaveNoise(p / _Scale), 2) * 5;
      }

      float SampleHeight2 (float2 p) {
        return OctaveNoise(p, 5);
      }

      float SampleHeight3 (float2 p) {
        return OctaveNoise(p, 3);
      }

      float island (float2 p, float a, float b) {
        return -pow(length(p), a) * b;
      }

      float4 frag (Fragment f) : SV_TARGET {
        float2 p = f.uv;
        float h = 0;
        h += 0.25 * OctaveNoise(p + 0.25 * OctaveNoise2(p + 0.25 * OctaveNoise2(p, 10), 10), 10);
        if (h > 0)
          h = pow(h, 2) * 5;
        else
          h /= 3;
        h += 0.002;
        h += smax(
          smax (
            island(p, 3.3, 1),
            island(p + float2(-0.3, 0.0), 2, 5)
          ),
          -0.1
        );
        return h;
      }

      ENDCG
    }
  }
  Fallback "Diffuse"
}
