#include "ixMath.Hash.cginc"

// Range [-1.0, 1.0]
// Non-uniform distribution
float noiseSimplex2D(float2 p)
{
	const float K1 = 0.366025404; // (sqrt(3)-1)/2;
	const float K2 = 0.211324865; // (3-sqrt(3))/6;

	float2  i = floor(p + (p.x + p.y) * K1);
	float2  a = p - i + (i.x + i.y) * K2;
	float m = step(a.y, a.x);
	float2  o = float2(m, 1.0 - m);
	float2  b = a - o + K2;
	float2  c = a - 1.0 + 2.0 * K2;
	float3  h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
	float3  n = h * h * h * h * float3(dot(a, hash2x2(i + 0.0)), dot(b, hash2x2(i + o)), dot(c, hash2x2(i + 1.0)));
//	return dot(n, float3(70.0, 70.0, 70.0));
	return dot(n, float3(73.529f, 73.529f, 73.529f)); // Scale to -1,1. original is 70.0f
}

// Noise2D from Unity Shader Graph
// Taken from Unity Shader Graph Unity_GradientNoise_Dir_float
float noise2D(float2 p)
{
	const float2 ip = floor(p);
	float2 fp = frac(p);
	float d00 = dot(grad2x2(ip), fp);
	float d01 = dot(grad2x2(ip + float2(0.0f, 1.0f)), fp - float2(0.0f, 1.0f));
	float d10 = dot(grad2x2(ip + float2(1.0f, 0.0f)), fp - float2(1.0f, 0.0f));
	float d11 = dot(grad2x2(ip + float2(1.0f, 1.0f)), fp - float2(1.0f, 1.0f));
	fp = fp * fp * fp * (fp * (fp * 6.0f - 15.0f) + 10.0f);
  
	return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5f;
}

float noise2Db(float2 p)
{
	uint2 i = uint2(floor(p));
	float2 f = frac(p);
	
	float2 u = f * f * (3.0f - 2.0f * f); // Cubic interpolation
//	float2 u = f * f * f * (f * (f * 6.0f - 15.0f) + 10.0f); // Quintic interpolation

	return lerp( lerp(	dot( grad2x2b(i + uint2(0,0) ),	f - float2(0.0f, 0.0f) ), 
						dot( grad2x2b(i + uint2(1,0) ), f - float2(1.0f, 0.0f) ), u.x),
				lerp(	dot( grad2x2b(i + uint2(0,1) ),	f - float2(0.0f, 1.0f) ), 
						dot( grad2x2b(i + uint2(1,1) ),	f - float2(1.0f, 1.0f) ), u.x), u.y);
}

// 3D Perlin Noise
// Scaled to -1, 1 (unscaled range is [-sqrt(3/4), sqrt(3/4)])
// Distribution non-uniform
float noise3D(in float3 p)
{
	const float3 i = floor(p);
	const float3 f = frac(p);
	const float3 u = f * f * (3.0 - 2.0 * f);

	return lerp(lerp(lerp(	dot(hash3x3(i), f),
							dot(hash3x3(i + float3(1.0, 0.0, 0.0)), f - float3(1.0, 0.0, 0.0)), u.x),
					lerp(	dot(hash3x3(i + float3(0.0, 1.0, 0.0)), f - float3(0.0, 1.0, 0.0)),
							dot(hash3x3(i + float3(1.0, 1.0, 0.0)), f - float3(1.0, 1.0, 0.0)), u.x), u.y),
				lerp(lerp(	dot(hash3x3(i + float3(0.0, 0.0, 1.0)), f - float3(0.0, 0.0, 1.0)),
							dot(hash3x3(i + float3(1.0, 0.0, 1.0)), f - float3(1.0, 0.0, 1.0)), u.x),
					lerp(	dot(hash3x3(i + float3(0.0, 1.0, 1.0)), f - float3(0.0, 1.0, 1.0)),
							dot(hash3x3(i + float3(1.0, 1.0, 1.0)), f - float3(1.0, 1.0, 1.0)), u.x), u.y), u.z)
			* 1.154700538f; // 1.0 / sqrt(3/4)
}
