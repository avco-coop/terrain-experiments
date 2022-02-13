Shader "Water" {
  Properties {
  }
  SubShader {
    Tags {
      "Queue" = "Transparent"
      "IgnoreProjector" = "True"
      "RenderType" = "Transparent"
      "DisableBatching" = "True"
    }
    Blend One OneMinusSrcAlpha
    ZWrite Off
    LOD 200
    
    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows alpha:fade addshadow
    #pragma target 3.0

    #include "Noise.cginc"
    
    sampler2D _CameraDepthTexture;
    
    struct Input {
      float3 worldPos;
      float4 screenPos;
    };
    
    UNITY_INSTANCING_BUFFER_START(Props)
    UNITY_INSTANCING_BUFFER_END(Props)
    
    float smootherstep(float x) {
      x = saturate(x);
      return saturate(x * x * x * (x * (6 * x - 15) + 10));
    }
    
    void surf (Input IN, inout SurfaceOutputStandard o)
    {
      float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
      depth = LinearEyeDepth(depth) - IN.screenPos.w;
      depth *= 5;
      o.Albedo = exp(-float3(0.9, 0.2, 0.1) * depth);
      o.Smoothness = 0.9;
      o.Alpha = 1 - saturate(exp(-0.1 * depth));
      // o.Normal = normalize(float3(
      //   OctaveNoise2(IN.worldPos.xz, 2) * 0.5 + 0.5,
      //   5
      // ));
      // float shoreDepth = smoothstep(0.0, _ShoreColorThreshold, Linear01Depth(depth));
      // depth = LinearEyeDepth(depth);
      // float foamDiff = smootherstep(saturate((depth - IN.screenPos.w) / _FoamIntersectionProperties.x));
      // float shoreDiff = smootherstep(saturate((depth - IN.screenPos.w) / _ShoreIntersectionThreshold));
      // float transparencyDiff = smootherstep(saturate((depth - IN.screenPos.w) / lerp(_TransparencyIntersectionThresholdMin, _TransparencyIntersectionThresholdMax, remap(sin(_Time.y + UNITY_PI / 2.0)))));
      // //Shore
      // float shoreFoam = saturate(foamTex - smoothstep(_FoamIntersectionProperties.y - _FoamIntersectionProperties.z, _FoamIntersectionProperties.y, foamDiff) + _FoamIntersectionProperties.w * (1.0 - foamDiff));
      // float sandWetness = smoothstep(0.0, 0.3 + 0.2 * remap(sin(_Time.y)), foamDiff);
      // shoreFoam *= sandWetness;
      // foam += shoreFoam;
      // //Colors
      // o.Albedo = lerp(lerp(fixed3(0.0, 0.0, 0.0), _ShoreColor.rgb, sandWetness), tex2D(_GradientMap, float2(IN.color.x, 0.0)).rgb, shoreDepth) + foam * sandWetness;
      // o.Emission = o.Albedo * saturate(_WorldSpaceLightPos0.y) * _LightColor0 * _Emission;
      //Smoothness
      // o.Smoothness = _Smoothness * foamDiff;
      // o.Alpha = saturate(lerp(1.0, lerp(0.5, _ShoreColor.a, sandWetness), 1.0 - shoreDiff) * transparencyDiff);
    }
    ENDCG
  }
  FallBack "Diffuse"
}