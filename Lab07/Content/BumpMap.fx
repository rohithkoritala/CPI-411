float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 CameraPosition;
float3 LightPosition;

float AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float4 SpecularColor;
float SpecularIntensity;
float Shininess;

texture normalMap;

sampler tsampler1 = sampler_state
{
	texture = <normalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
};

struct VertexShaderInput {

	float4 Position: SV_Position0;
	float2 TexCoord: TEXCOORD0;
	float4 Normal: NORMAL0;
	float4 Tangent: TANGENT0;
	float4 Binormal: BINORMAL0;
};

struct VertexShaderOutput {

	float4 Position: POSITION0;
	float3 Normal: TEXCOORD0;
	float3 Tangent: TEXCOORD1;
	float3 Binormal: TEXCOORD2;
	float2 TexCoord: TEXCOORD3;
	float3 Position3D: TEXCOORD4;
};

PhongVertexOutput PhongVertex(PhongVertexInput input)
{
	PhongVertexOutput output;
	// Do the transformations as before
	// Save the world position for use in the pixel shader
	output.WorldPosition = mul(input.Position, World);
	float4 viewPosition = mul(output.WorldPosition, View);
	output.Position = mul(viewPosition, Projection);
	// as well as the normal in world space
	output.WorldNormal = mul(input.Normal, World);
	output.UV = input.UV * 10;
	return output;
}
// The pixel shader performs the lighting
float4 PhongPixel(PhongVertexOutput input) : COLOR0
{
	// The lighting operation, same as in the Gouraud vertex method
	float3 lightDirection = normalize(LightPosition - input.WorldPosition.xyz);
	float3 viewDirection = normalize(CameraPosition - input.WorldPosition.xyz);
	// Need to normalize my incoming normal, length could be less than 1
	input.WorldNormal = normalize(input.WorldNormal);
	float3 reflectDirection = -reflect(lightDirection, input.WorldNormal);
	// Now, compute the lighint components
	float4 diffuse = max(dot(lightDirection, input.WorldNormal), 0) * tex2D(DiffuseSampler, input.UV);
	float specular = pow(max(dot(reflectDirection, viewDirection), 0), Shininess);
	return float4(AmbientColor + diffuse.x * DiffuseColor + specular * SpecularColor, 1);
}


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 projPosition = mul(viewPosition, Projection);
	output.Position = projPosition;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
	output.Position3D = worldPosition.xyz;
	output.TexCoord = input.TexCoord;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input):COLOR0
{
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 H = normalize(L + V);
	float3 normalTex = (tex2D(tsampler1, input.TexCoord).xyz - float3(0.5, 0.5, 0.5))*2.0;
	float3 bumpNormal = input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal;
	float3 diffuse = dot(bumpNormal, L);
	//float4 specular = pow(saturate(dot(H, bumpNormal)), Shininess);
	//return diffuse + specular;
	return float4(diffuse, 1);
	//return tex2D(tsampler1, input.TexCoord);

	/*float3x3 TBN;
	TBN[0] = T;
	TBN[1] = B;
	TBN[2] = N;

	float3 bumpNormal = mul(normalTex, TBN);*/


}

technique bump
{
	pass Pass1 {
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}

technique bump
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertex();
		PixelShader = compile ps_4_0 PhongPixel();
	}
}