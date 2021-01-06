float4x4 MatrixTransform;
texture2D modelTexture;
float imageWidth;
float imageHeight;
texture2D filterTexture;
sampler TextureSampler: register(s0) = sampler_state {
	Texture = <modelTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VS_OUTPUT {
	float4 Pos : POSITION;
	float2 UV0: TEXCOORD0;
	float4 UV1: TEXCOORD1;
};