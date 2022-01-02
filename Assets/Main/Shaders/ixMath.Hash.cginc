// Hash functions
// https://www.shadertoy.com/view/XlGcRh
// http://www.jcgt.org/published/0009/03/02/
// 
// https://nullprogram.com/blog/2018/07/31/

// Integer Hash - I
// - Inigo Quilez, Integer Hash - I, 2017
//   https://www.shadertoy.com/view/llGSzw
uint iq1to1(uint n)
{
    // integer hash copied from Hugo Elias
    n = (n << 13U) ^ n;
    n = n * (n * n * 15731U + 789221U) + 1376312589U;

    return n;
}

// Integer Hash - III
// - Inigo Quilez, Integer Hash - III, 2017
//   https://www.shadertoy.com/view/4tXyWN
uint iq2to1(uint2 x)
{
    uint2 q = 1103515245U * ((x >> 1U) ^ (x.yx));
    uint  n = 1103515245U * ((q.x) ^ (q.y >> 3U));

    return n;
}

// Integer Hash - II
// - Inigo Quilez, Integer Hash - II, 2017
//   https://www.shadertoy.com/view/XlXcW4
uint3 iq3to3(uint3 x)
{
    const uint k = 1103515245u;

    x = ((x >> 8U) ^ x.yzx) * k;
    x = ((x >> 8U) ^ x.yzx) * k;
    x = ((x >> 8U) ^ x.yzx) * k;

    return x;
}

// https://www.pcg-random.org/
uint pcg1to1(uint v)
{
    uint state = v * 747796405u + 2891336453u;
    uint word = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
    return (word >> 22u) ^ word;
}

uint2 pcg2to2(uint2 v)
{
    v = v * 1664525u + 1013904223u;

    v.x += v.y * 1664525u;
    v.y += v.x * 1664525u;

    v = v ^ (v >> 16u);

    v.x += v.y * 1664525u;
    v.y += v.x * 1664525u;

    v = v ^ (v >> 16u);

    return v;
}

// http://www.jcgt.org/published/0009/03/02/
uint3 pcg3to3(uint3 v) {

    v = v * 1664525u + 1013904223u;

    v.x += v.y * v.z;
    v.y += v.z * v.x;
    v.z += v.x * v.y;

    v ^= v >> 16u;

    v.x += v.y * v.z;
    v.y += v.z * v.x;
    v.z += v.x * v.y;

    return v;
}

uint xxhash32(uint3 p)
{
    const uint PRIME32_2 = 2246822519U, PRIME32_3 = 3266489917U;
    const uint PRIME32_4 = 668265263U, PRIME32_5 = 374761393U;
    uint h32 = p.z + PRIME32_5 + p.x * PRIME32_3;
    h32 = PRIME32_4 * ((h32 << 17) | (h32 >> (32 - 17)));
    h32 += p.y * PRIME32_3;
    h32 = PRIME32_4 * ((h32 << 17) | (h32 >> (32 - 17)));
    h32 = PRIME32_2 * (h32 ^ (h32 >> 15));
    h32 = PRIME32_3 * (h32 ^ (h32 >> 13));
    return h32 ^ (h32 >> 16);
}

// Return range: -1 to 1
// Distribution: Uniform
// Note, hash2x2(float3(0,0)) return float3(-1,-1)
float2 hash2x2(float2 p) // replace this by something better
{
	p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
	return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}
// hash
// Return range: -1 to 1
// Distribution: Uniform
// Note, hash(float3(0,0,0)) return float3(-1,-1,-1)
float3 hash3x3(float3 p) // replace this by something better
{
    p = float3(dot(p, float3(127.1, 311.7, 74.7)),
        dot(p, float3(269.5, 183.3, 246.1)),
        dot(p, float3(113.5, 271.9, 124.6)));

    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

uint hash1to1(uint seed) { return iq1to1(seed); }; // pcg1to1 or iq1to1
uint hash2to1(uint2 seed) { return iq2to1(seed); }; // iq2to1
uint hash3to1(uint3 seed) { return xxhash32(seed); };
uint2 hash2to2(uint2 seed) { return pcg2to2(seed); }; // pcg2to2
uint3 hash3to3(uint3 seed) { return iq3to3(seed); }; // pcg3to3 or iq3to3

float2 grad2x2(float2 p)
{
	// Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
	p = p % 289;
	// need full precision, otherwise half overflows when p > 1
	float x = (34 * p.x + 1.0f) * p.x % 289 + p.y;
	x = (34 * x + 1) * x % 289;
	x = frac(x / 41) * 2 - 1;
	return normalize(float2(x - floor(x + 0.5f), abs(x) - 0.5f));
}


// MIT License Copyright © 2013 Inigo Quilez
// From https://www.shadertoy.com/view/XdXGW8
float2 grad2x2b(uint2 z )  // replace this anything that returns a random vector
{
	// 2D to 1D  (feel free to replace by some other)
	int n = z.x+z.y*11111;

	// Hugo Elias hash (feel free to replace by another one)
	n = (n<<13)^n;
	n = (n*(n*n*15731+789221)+1376312589)>>16;

	// Perlin style vectors
	n &= 7;
	float2 gr = float2(n&1,n>>1)*2.0-1.0;
	return ( n>=6 ) ? float2(0.0,gr.x) : 
		   ( n>=4 ) ? float2(gr.x,0.0) :
							  gr;
}