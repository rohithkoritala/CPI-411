

texture MyTexture;
float4x4 View;
float4x4 Projection;
float4x4 World;
float3 offset;
sampler mySampler = sampler_state { Texture = <MyTexture>; };

struct VertexPositionColor {
	float4 Position: POSITION;
	float4 Color: COLOR;
};

struct VertexPositionTexture
{
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};




VertexPositionTexture MyVertexShader(VertexPositionTexture input) 
{
	//input.Position.xyz += offset;
	//return input;
	VertexPositionTexture output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	//float2  tex = input.TextureCoordinate;
	return tex2D(mySampler, input.TextureCoordinate);
	
}

/*float4 MyVertexShader(float4 position: POSITION) : POSITION
{
return position;
}
float4 MyPixelShader() : COLOR
{
return float4(1, 1, 1, 0.5f);
} */
technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}
