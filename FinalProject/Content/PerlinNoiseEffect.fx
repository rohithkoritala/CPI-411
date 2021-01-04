﻿float4x4 World;
float4x4 View;
float4x4 Projection;
float3 LightPosition;
float inz;
float height = 1.0f;
float E;
float fact= 0.5f;
float noise = 0.5f;
float Ai = 0.3f;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	float3 Normal	: NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	float4 wPosition: TEXCOORD1;
	float3 Light	: TEXCOORD2;
	float3 Normal	: NORMAL;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.wPosition = mul(input.Position, World);
	output.texCoord = input.texCoord;
	output.Light = normalize(LightPosition);
	output.Normal = normalize(mul(input.Normal, World));

	return output;
}

texture permTexture2d;
texture permGradTexture;

sampler permSampler2d = sampler_state
{
	texture = <permTexture2d>;
	AddressU = Wrap;
	AddressV = Wrap;
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = NONE;
};

sampler permGradSampler = sampler_state
{
	texture = <permGradTexture>;
	AddressU = Wrap;
	AddressV = Clamp;
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = NONE;
};

float3 fade(float3 t)
{
	return t * t * t * (t * (t * 6 - 15) + 10); // new curve
}
float4 perm2d(float2 p)
{
	return tex2D(permSampler2d, p);
}
float gradperm(float x, float3 p)
{
	return dot(tex1D(permGradSampler, x), p);
}

float inoise(float3 p)
{
	float3 P = fmod(floor(p), 256.0);	// FIND UNIT CUBE THAT CONTAINS POINT
	p -= floor(p);                      // FIND RELATIVE X,Y,Z OF POINT IN CUBE.
	float3 f = fade(p);                 // COMPUTE FADE CURVES FOR EACH OF X,Y,Z.

	P = P / 256.0;
	const float one = 1.0 / 256.0;

	// HASH COORDINATES OF THE 8 CUBE CORNERS
	float4 AA = perm2d(P.xy) + P.z;


	// AND ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
	return lerp(lerp(lerp(gradperm(AA.x, p),
		gradperm(AA.z, p + float3(-1, 0, 0)), f.x),
		lerp(gradperm(AA.y, p + float3(0, -1, 0)),
			gradperm(AA.w, p + float3(-1, -1, 0)), f.x), f.y),

		lerp(lerp(gradperm(AA.x + one, p + float3(0, 0, -1)),
			gradperm(AA.z + one, p + float3(-1, 0, -1)), f.x),
			lerp(gradperm(AA.y + one, p + float3(0, -1, -1)),
				gradperm(AA.w + one, p + float3(-1, -1, -1)), f.x), f.y), f.z);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 p = input.wPosition;
	inz = inoise(p)*0.5 + fact;
	float4 color = float4(inz,inz,inz,1);

	E = 0.001f;
	
	float3 pX = input.wPosition;
	pX.x += E * height;
	float3 pY = input.wPosition;
	pY.y += E * height;
	float3 pZ = input.wPosition;
	pZ.z += E * height;
	float3 bump = float3(inoise(pX)*0.5 + noise,inoise(pY)*0.5 + noise,inoise(pZ)*0.5 + noise);


	float3 modNormal = float3((bump.x - inz) / E, (bump.y - inz) / E, (bump.z - inz) / E);
	float3 Normal = normalize(input.Normal - modNormal);

	//float Ai = 0.3f;
	float4 Ac = float4(0.3, 0.0, 0.3, 1.0);
	float Di = 1.0f;
	float4 Dc = float4(1.0, 1.0, 1.0, 1.0);
	return Ai * Ac + color * saturate(dot(input.Light, Normal));
}

technique PerlinNoise
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}