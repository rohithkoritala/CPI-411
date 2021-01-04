﻿#include "inoise.fxh"

//------------------------------------
float4x4 worldViewProj : WorldViewProjection;
float4x4 world : World;
float4x4 View;

string description = "Perlin noise test";

float noiseScale
<
	string UIWidget = "slider";
	string UIName = "noise scale";
	float UIMin = 0.0; float UIMax = 20.0; float UIStep = 0.01;
> = 5.0;

float anim
<
	string UIWidget = "slider";
	string UIName = "animate";
	float UIMin = 0.0; float UIMax = 10.0; float UIStep = 0.01;
> = 0.0;

float lacunarity
<
	string UIWidget = "slider";
	string UIName = "lacunarity";
	float UIMin = 0.0; float UIMax = 10.0; float UIStep = 0.01;
> = 2.0;

float gain
<
	string UIWidget = "slider";
	string UIName = "gain";
	float UIMin = 0.0; float UIMax = 1.0; float UIStep = 0.01;
> = 0.5;

float threshold
<
	string UIWidget = "slider";
	string UIName = "threshold";
	float UIMin = 0.0; float UIMax = 1.0; float UIStep = 0.01;
> = 0.5;

float transition_width
<
	string UIWidget = "slider";
	string UIName = "transition width";
	float UIMin = 0.0; float UIMax = 1.0; float UIStep = 0.01;
> = 0.05;

float4 color1 : DIFFUSE
<
	string UIName = "color 1";
> = float4(0.0, 0.0, 0.5, 1.0);

float4 color2 : DIFFUSE
<
	string UIName = "color 2";
> = float4(0.0, 0.7, 0.0, 1.0);

//------------------------------------
struct vertexInput {
	float4 position		: POSITION;
	float2 texcoord     : TEXCOORD;
};

struct vertexOutput {
	float4 hPosition		: POSITION;
	float2 texcoord      : TEXCOORD0;
	float3 wPosition		: TEXCOORD1;
};


//------------------------------------
vertexOutput VS(vertexInput IN)
{
	vertexOutput OUT;
	float4 worldPosition = mul(IN.position, world);
	float4 viewPosition = mul(worldPosition, View);
	OUT.hPosition = mul(viewPosition, worldViewProj);
	OUT.texcoord = IN.texcoord;// * noiseScale;
	OUT.wPosition = mul(IN.position, world).xyz;// * noiseScale;
	return OUT;
}

//-----------------------------------
float4 PS_inoise(vertexOutput IN) : COLOR
//float4 PS_inoise(float2 p : TEXCOORD): COLOR
{
	//float3 p = IN.wPosition;
	//return inoise(p)*0.5+0.5;
	//float3 pos = float3(p * 10, 0);
	//return inoise(pos)*0.5+0.5;
	return inoise(float3(IN.texcoord * 10, 0.0))*0.5 + 0.5;
}

float4 PS_inoise4d(vertexOutput IN) : COLOR
{
	float3 p = IN.wPosition;
	return inoise(float4(p, anim))*0.5 + 0.5;
	//	return abs(inoise(float4(p, anim)));
}

//float4 PS_fBm(float2 p : TEXCOORD): COLOR
float4 PS_fBm(vertexOutput IN) : COLOR
{
	float3 p = IN.wPosition;
	return fBm(float3(IN.texcoord, 0), 4, lacunarity, gain)*0.5 + 0.5;
	//float3 pos = float3(p * 10, 0);
	//return fBm(pos, 4)*0.5+0.5;
}

float4 PS_earth(vertexOutput IN) : COLOR
{
	float3 p = IN.wPosition;
	float n = fBm(p, 4, lacunarity, gain)*0.5 + 0.5;
	return lerp(color1, color2, smoothstep(threshold - transition_width, threshold + transition_width, n));
}

float4 PS_turbulence(vertexOutput IN) : COLOR
{
	float3 p = IN.wPosition;
	return turbulence(p, 4, lacunarity, gain);
}

float4 PS_ridgedmf(vertexOutput IN) : COLOR
{
	float3 p = IN.wPosition;
	return ridgedmf(p, 4, lacunarity, gain);
}

//-----------------------------------
technique inoise
{
	pass p0
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PS_inoise();
	}
}