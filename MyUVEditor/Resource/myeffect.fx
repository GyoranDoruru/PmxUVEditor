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

struct  VS_OUTPUT{
	float4 Pos		: SV_Position;
	float2 Tex		: TEXCOORD1;
	float3 Normal	: TEXCOORD2;
	float3 Eye		: TEXCOORD3;
	float4 Color	: COLOR0;
	float3 Specular	: COLOR1;
};

VS_OUTPUT MyVertexShader(float4 Pos : SV_POSITION, float3 Normal : NORMAL, float2 Tex : TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;
	Out.Pos = mul(Pos, WorldViewProjMatrix);
	Out.Tex = Tex;
	Out.Normal = mul(Normal, (float3x3)WorldMatrix);
	Out.Eye = CameraPosition - mul(Pos, WorldMatrix);
	Out.Color.rgb = saturate(max(0, dot(Out.Normal, -LightDirection))*DiffuseColor.rgb + AmbientColor);
	Out.Color.a = DiffuseColor.a;
	float3 HalfVector = normalize(normalize(Out.Eye) - LightDirection);
		Out.Specular = pow(max(0, dot(HalfVector, Out.Normal)), SpecularPower)*SpecularColor;
	return Out;
}

float4 MyPixelShader(VS_OUTPUT input) : SV_Target
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