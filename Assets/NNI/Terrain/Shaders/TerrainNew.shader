Shader "Terrain New" {

  Properties {
    _Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
  }

  SubShader {

    Pass {
      CGPROGRAM
      #pragma vertex MyVertexProgram
      #pragma fragment MyFragmentProgram

      #include "UnityCG.cginc"
      
      float4 _Tint;

			sampler2D _MainTex;

      struct Interpolators {
        float4 position : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
      };
      
      struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
			};

      Interpolators MyVertexProgram (VertexData v) {
        Interpolators i;
        i.position = UnityObjectToClipPos(v.position);
				i.normal = UnityObjectToWorldNormal(v.normal);
        i.uv = v.uv;
        return i;
      }

      float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
				i.normal = normalize(i.normal);
				return float4(i.normal * 0.5 + 0.5, 1);
      }

      ENDCG
    }

  }

}
