// *** CPI411 Lab#7 (BumpMap) 
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
float3 LightPosition;
// Light Uniforms
float AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float4 SpecularColor;
float SpecularIntensity;
float Shininess;
// Bump Mapping Uniforms
float height = 2.0f;
float2 UVScale = float2(1.0f, 1.0f);
texture normalMap;
Texture decalMap;
Texture environmentMap;
//
float NormalMapRepeatU;  //repeat rate of U for normal map
float NormalMapRepeatV;  //repeat rate of V for normal map
int SelfShadow;  //0:nope 1:step2 2:step3
float BumpHeight;  //how bumpy the bump mapping is [1~10]
int NormalizeTangentFrame;  //0:nope 1:yes
int NormalizeNormalMap;
//0:nope 1:for both diffuse and specular 2:for specular only
//3:for specular only
int MipMap;  //0:normal 1:bad(off)



sampler tsampler1 = sampler_state {
	texture = <normalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Clamp; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Clamp;
};
samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <environmentMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

bool useSelfShadowing = false;
struct VertexShaderInput {
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 Tangent : TANGENT0;
	float4 Binormal : BINORMAL0;
	float2 TexCoord : TEXCOORD0;
};
struct VertexShaderOutput {
	float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float3 Tangent : TEXCOORD1;
	float3 Binormal : TEXCOORD2;
	float2 TexCoord : TEXCOORD3;
	float3 Position3D : TEXCOORD4;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	// TODO: add your vertex shader code here./* // to transform from object space to tangent space
	float3x3 objectToTangentSpace;
	objectToTangentSpace[0] = input.Tangent;
	objectToTangentSpace[1] = input.Binormal;
	objectToTangentSpace[2] = input.Normal;
	// [ Tx, Ty, Tz ] [ Objx ] = [ Tanx ]
	// [ Bx, By, Bz ] [ Objy ] = [ Tany ]
	// [ Nx, Ny, Nz ] [ Objz ] = [ Tanz ]
	output.Normal = mul(objectToTangentSpace, input.Normal);
	// object -> tangent?
	output.Tangent = mul(objectToTangentSpace, input.Tangent);
	output.Binormal = mul(objectToTangentSpace, input.Binormal);
	//*/// World Space looks better 
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
	float3 N = output.Normal;
	float3 I = normalize(worldPosition.xyz - CameraPosition);
	//output.Tangent = reflect(I, N);
	output.Position3D = worldPosition.xyz;
	output.TexCoord = input.TexCoord;
	//* UVScale;
	return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Vectors: L = Light; V = View; N = Normal; T = Tangent; 
	//B = Binormal; H = Halfway between L and V
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3 B = normalize(input.Binormal);
	float3 H = normalize(L + V);
	float3 I = normalize(input.Position3D - CameraPosition);
	// Calculate the normal, including the information in the bump map
	float3 normalTex = tex2D(tsampler1, input.TexCoord).xyz;
	normalTex =  2.0 * (normalTex - 0.5);

		normalTex.x *= (1 + 0.2*(height - 5));
		normalTex.y *= (1 + 0.2*(height - 5));
		normalTex.z *= (1 + 0.2*(5 - height));
	//expand(normalTex);// *** Lab7 ********//
	//float3 bumpNormal = normalize(N + (normalTex.x * T + normalTex.y * B));
	// (Lab7 Option MonoGame3.4) // If does not work, use the OPTION-A//
	//float3 bumpNormal = normalize(N + (normalTex.x * float3(1, 0, 0) + normalTex.y * float3(0, 1, 0))); 
	// OPTION A// *** for Assignment3 ***
	float3x3 TangentToWorld;
	TangentToWorld[0] = (input.Tangent);
	TangentToWorld[1] = (input.Binormal);
	TangentToWorld[2] = (input.Normal);
	float3 bumpNormal = mul(normalTex, TangentToWorld);
	float3 R = reflect(I, bumpNormal);
	//calculate Diffuse Term:
	float4 diffuse = DiffuseColor * DiffuseIntensity * max(0, (dot(bumpNormal, L)));
	diffuse.a = 1.0;
	// calculate Specular Term (H,N):
	float4 specular = SpecularColor * SpecularIntensity * pow(saturate(dot(H, bumpNormal)), Shininess);
	specular.a = 1.0;
	// Compute Final Color
	float4 finalColor = diffuse + specular;
	//ambient + diffuse + specular;
	float4 reflectedColor = texCUBE(SkyBoxSampler, R);
	//float4 decalColor = tex2D(tsampler1, input.TexCoord);
	return reflectedColor; // lerp(decalColor, reflectedColor, 0.5);

	//return finalColor;
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}