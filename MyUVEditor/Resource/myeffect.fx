Texture2D diffuseTexture   : MATERIALTEXTURE;
float4x4 WorldMatrix              : WORLD;
float4x4 WorldViewProjMatrix      : WORLDVIEWPROJECTION;
float4x4 LightWorldViewProjMatrix : WORLDVIEWPROJECTION < string Object = "Light"; >;

float3   LightDirection    : DIRECTION < string Object = "Light"; >;
float3   CameraPosition    : POSITION  < string Object = "Camera"; >;

// マテリアル色
float4   MaterialDiffuse   : DIFFUSE  < string Object = "Geometry"; >;
float3   MaterialAmbient   : AMBIENT  < string Object = "Geometry"; >;
float3   MaterialEmmisive  : EMISSIVE < string Object = "Geometry"; >;
float3   MaterialSpecular  : SPECULAR < string Object = "Geometry"; >;
float    SpecularPower : SPECULARPOWER < string Object = "Geometry"; >;
float3   MaterialToon      : TOONCOLOR;
// ライト色
float3   LightDiffuse      : DIFFUSE   < string Object = "Light"; >;
float3   LightAmbient      : AMBIENT   < string Object = "Light"; >;
float3   LightSpecular     : SPECULAR  < string Object = "Light"; >;
static float4 DiffuseColor = MaterialDiffuse  * float4(LightDiffuse, 1.0f);
static float3 AmbientColor = MaterialAmbient  * LightAmbient + MaterialEmmisive;
static float3 SpecularColor = MaterialSpecular * LightSpecular;
SamplerState mySampler{

};

struct PmxVertexStruct{
	float4 Position :SV_Position;
	float2 Tex : TEXCOORD;
};

PmxVertexStruct MyVertexShader(PmxVertexStruct input)
{
	PmxVertexStruct output = input;
	output.Position = mul(output.Position, WorldViewProjMatrix);
	return output;
}

float4 MyPixelShader(PmxVertexStruct input) : SV_Target
{
	return MaterialDiffuse;
	//return diffuseTexture.Sample(mySampler, input.Tex);
}

technique10 MyTechnique
{
	pass MyPass
	{
		SetVertexShader(CompileShader(vs_5_0, MyVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader()));
	}
}