float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4 AmbientColor;
float AmbientIntensity;
float3 DiffuseLightDirection;
float4 DiffuseColor;
float DiffuseIntensity;


float3 LightPosition;


float3 CameraPosition;
float Shininess;
float4 SpecularColor;
float specularIntensity = 1;

struct VertexInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct VertexShaderOutput{
	float4 Position : POSITION;
	float4 Color : COLOR;
	float4 Normal : TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};


VertexShaderOutput GouraudVertexShaderFunction(VertexInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = 0;
	output.Normal = 0;
	float3 N = mul(input.Normal,WorldInverseTranspose).xyz;
	float3 V = normalize(CameraPosition - worldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * specularIntensity;
	output.Color = saturate(ambient + diffuse + specular);
	return output;

}

VertexShaderOutput PhongVertexShaderFunction(VertexInput input)
{
		VertexShaderOutput output;
		float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
		output.WorldPosition = worldPosition;
		output.Normal = input.Normal;

		return output;	
}

float4 GouraudPixelShaderFunction(VertexShaderOutput input) :COLOR
{
	return input.Color;
}

float4 PhongPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * specularIntensity;
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;
	return color;
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 GouraudVertexShaderFunction();
		PixelShader = compile ps_4_0 GouraudPixelShaderFunction();
	}
}
technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 PhongPixelShaderFunction();
	}
}
