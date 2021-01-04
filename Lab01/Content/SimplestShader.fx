

texture MyTexture;
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
return input;
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
