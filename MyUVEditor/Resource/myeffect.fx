float4x4 WorldMatrix              : WORLD;
float4x4 ViewMatrix               : VIEW;
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

Texture2D ObjectTexture		: MATERIALTEXTURE;
SamplerState ObjTexSampler{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D ObjectSphereMap: MATERIALSPHEREMAP;
SamplerState ObjSphareSampler{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D ObjectToonTexture: MATERIALTOONTEXTURE;
SamplerState ObjToonSampler{
	Filter = MIN_MAG_LINEAR_MIP_POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};


struct  VS_OUTPUT{
	float4 Pos		: SV_Position;
	float2 Tex		: TEXCOORD1;
	float3 Normal	: TEXCOORD2;
	float3 Eye		: TEXCOORD3;
	float2 SpTex	: TexCoord4;
	float4 Color	: COLOR0;
	float3 Specular	: COLOR1;
};

VS_OUTPUT MyVertexShader(float4 Pos : SV_POSITION, float3 Normal : NORMAL, float2 Tex : TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;
	Out.Pos = mul(Pos, WorldViewProjMatrix);
	Out.Tex = Tex;
	Out.Normal = normalize(mul(Normal, (float3x3)WorldMatrix));
	Out.Eye = CameraPosition - mul(Pos, WorldMatrix);

	// Use SubTexture
	//	Out.SpTex = Tex2;
	// Use SphereTexture
	float2 NormalWV = mul(Out.Normal, (float3x3)ViewMatrix).xy;
	Out.SpTex.x = NormalWV.x * 0.5f + 0.5f;
	Out.SpTex.y = NormalWV.y * -0.5f + 0.5f;

	Out.Color.rgb = AmbientColor;
	Out.Color.rgb += max(0, dot(Out.Normal, -LightDirection))*DiffuseColor.rgb;
	Out.Color.a = DiffuseColor.a;
	Out.Color = saturate(Out.Color);
	float3 HalfVector = normalize(normalize(Out.Eye) - LightDirection);
	Out.Specular = pow(max(0, dot(HalfVector, Out.Normal)), SpecularPower)*SpecularColor;
	return Out;
}

float4 MyPixelShader(VS_OUTPUT IN, uniform bool isSph) : SV_Target
{
	// material
	float4 Color = IN.Color;
	// texture
	Color *= ObjectTexture.Sample(ObjTexSampler, IN.Tex);
	// sphere
	float4 TexColor = ObjectSphereMap.Sample(ObjSphareSampler, IN.SpTex);
	if (isSph) Color.rgb *= TexColor.rgb;
	else Color.rgb += TexColor.rgb;

	Color.a *= TexColor.a;
	// toon
	float LightNormal = dot(IN.Normal, -LightDirection);
	Color *= ObjectToonTexture.Sample(ObjToonSampler, float2(0, 0.5f - LightNormal*0.5f));

	// specular
	Color.rgb += IN.Specular;

	return Color;
}

technique10 SphTechnique
{
	pass DrawObject
	{
		SetVertexShader(CompileShader(vs_5_0, MyVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader(true)));
	}
}

technique10 SpaTechnique
{
	pass DrawObject
	{
		SetVertexShader(CompileShader(vs_5_0, MyVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader(false)));
	}
}

// Sprite

struct  SPRITE_OUTPUT{
	float4 Pos		: SV_Position;
	float2 Tex		: TEXCOORD1;
};

SPRITE_OUTPUT SpriteVertexShader(float4 Pos : SV_POSITION)
{
	SPRITE_OUTPUT Out = (SPRITE_OUTPUT)0;
	Out.Pos = mul(Pos, WorldViewProjMatrix);
	Out.Tex = Pos.xy;
	return Out;
}

float4 SpritePixelShader(SPRITE_OUTPUT IN) : SV_Target
{
	// material
	float4 Color = float4(1,1,1,1);
	Color *= ObjectTexture.Sample(ObjTexSampler, IN.Tex);
	return Color;
}

technique10 SpriteTechnique
{
	pass DrawObject
	{
		SetVertexShader(CompileShader(vs_5_0, SpriteVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, SpritePixelShader()));
	}
}
