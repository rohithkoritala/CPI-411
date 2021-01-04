float4x4  World;
float4x4  View;
float4x4  Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor;
float AmbientIntensity;
float3 DiffuseLightDirection;
float4 DiffuseColor;
float DiffuseIntensity;

float3 CameraPosition;
float3 LightPosition;

float Shininess;
float4 SpecularColor;
float SpecularIntensity = 1;

struct VertexShaderInput
{
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct GouraudVertexShaderOutput
{
	float4 Position: POSITION;
	float4 Color: COLOR;
};

struct PhongVertexShaderOutput
{
	float4 Position: POSITION;
	float4 Normal: TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

struct ToonVertexShaderOutput
{
	float4 Position: POSITION;
	float4 Normal: TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

GouraudVertexShaderOutput GouraudVertex(VertexShaderInput input)
{
	GouraudVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - worldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L, N);

	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	output.Color = saturate(ambient + diffuse + specular);
	return output;
}

PhongVertexShaderOutput PhongVertex(VertexShaderInput input) 
{
	PhongVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

ToonVertexShaderOutput ToonVertex(VertexShaderInput input) 
{
	ToonVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

PhongVertexShaderOutput PhongBlinnVertex(VertexShaderInput input) 
{
	PhongVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

PhongVertexShaderOutput SchlickVertex(VertexShaderInput input) 
{
	PhongVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

PhongVertexShaderOutput HalfLifeVertex(VertexShaderInput input) 
{
	PhongVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;

	return output;
}

float4 GouraudPixel(GouraudVertexShaderOutput input):COLOR
{
	return input.Color;
}

float4 PhongPixel(PhongVertexShaderOutput input):COLOR0
{
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L,N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	float4 specular = pow(max(0,dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;
	return color;
}

float4 ToonPixel(ToonVertexShaderOutput input) :COLOR0
{
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L,N);
	float D = dot(V, R);
	if (D < -0.7)
	{
		return float4(0, 0, 0, 1);
	}
	else if (D < 0.2)
	{
		return float4(0.25, 0.25, 0.25, 1);
	}
	else if (D < 0.97)
	{
		return float4(0.5, 0.5, 0.5, 1);
	}
	else
	{
		return float4(1, 1, 1, 1);
	}
}

float4 PhongBlinnPixel(PhongVertexShaderOutput input) :COLOR0
{
	float4 color = float4(0,0,0,0);
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L,N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	color += ambient + diffuse;
	float3 H = normalize(L + V);

	color += SpecularColor * SpecularIntensity * max(0, pow(max(0,dot(H,N)), Shininess));
	return color;
}

float4 SchlickPixel(PhongVertexShaderOutput input): COLOR0
{
	float4 color = float4(0,0,0,0);
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	color += ambient + diffuse;
	float4 t = max(0, dot(V, R));
	color += SpecularColor * SpecularIntensity * t / (Shininess - t * Shininess + t);
	return color;
}

float4 HalfLifePixel(PhongVertexShaderOutput input): COLOR0
{
	float4 color = float4(0,0,0,0);
	float3 L = normalize(LightPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 N = mul(input.Normal, WorldInverseTranspose).xyz;
	float3 R = reflect(-L, N);

	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor* (0.5 * pow(dot(N, L), 2));

	color += ambient + diffuse;
	float4 t = max(0, dot(V, R));

	color += SpecularColor * SpecularIntensity * t / (Shininess - t * Shininess + t);
	return color;
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 GouraudVertex();
		PixelShader = compile ps_4_0 GouraudPixel();
	}
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertex();
		PixelShader = compile ps_4_0 PhongPixel();
	}
}
technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongBlinnVertex();
		PixelShader = compile ps_4_0 PhongBlinnPixel();
	}
}
technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 SchlickVertex();
		PixelShader = compile ps_4_0 SchlickPixel();
	}
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 ToonVertex();
		PixelShader = compile ps_4_0 ToonPixel();
	}
}
technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 HalfLifeVertex();
		PixelShader = compile ps_4_0 HalfLifePixel();
	}
}