#pragma exclude_renderers gles

SamplerState sampler_linear_repeat; 

#ifdef _TILE_STOCHASTIC

float2 hash2D2D (float2 s)
{
	return frac(sin(fmod(float2(dot(s, float2(127.1,311.7)), dot(s, float2(269.5,183.3))), 3.14159))*43758.5453);
}

float4 tex2DStochastic(DEFTEXTURE(tex), float2 uv)
{
	float4x3 BW_vx;
	float2 skewUV = mul(float2x2(1.0, 0.0, -0.57735027, 1.15470054), uv * 3.464);
	float2 vxID = float2(floor(skewUV));
	float3 barry = float3(frac(skewUV), 0);
	barry.z = 1.0 - barry.x - barry.y;
	BW_vx = ((barry.z>0) ? 
		float4x3(float3(vxID, 0), float3(vxID + float2(0, 1), 0), float3(vxID + float2(1, 0), 0), barry.zyx) :
		float4x3(float3(vxID + float2 (1, 1), 0), float3(vxID + float2 (1, 0), 0), float3(vxID + float2 (0, 1), 0), float3(-barry.z, 1.0-barry.y, 1.0-barry.x)));
	float2 dx = ddx(uv);
	float2 dy = ddy(uv);
	return mul(TEXTUREGRAD(tex, uv + hash2D2D(BW_vx[0].xy), dx, dy), BW_vx[3].x) + 
			   mul(TEXTUREGRAD(tex, uv + hash2D2D(BW_vx[1].xy), dx, dy), BW_vx[3].y) + 
			   mul(TEXTUREGRAD(tex, uv + hash2D2D(BW_vx[2].xy), dx, dy), BW_vx[3].z);
}

#else

float4 tex2DStochastic(DEFTEXTURE(tex), float2 uv)
{
  return TEXTURE(tex, uv);
}

#endif
