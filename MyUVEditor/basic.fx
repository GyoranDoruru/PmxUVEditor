////////////////////////////////////////////////////////////////////////////////////////////////
//
//  basic.fx ver2.0
//  作成: 舞力介入P
//
////////////////////////////////////////////////////////////////////////////////////////////////
// パラメータ宣言

// 座法変換行列
float4x4 WorldViewProjMatrix      : WORLDVIEWPROJECTION;
float4x4 WorldMatrix              : WORLD;
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

bool use_texture;  //テクスチャの有無
bool use_toon;     //トゥーンの有無

bool     parthf;   // パースペクティブフラグ
bool     transp;   // 半透明フラグ
#define SKII1    1500
#define SKII2    8000
#define Toon     3

// オブジェクトのテクスチャ
texture ObjectTexture: MATERIALTEXTURE;
sampler ObjTexSampler = sampler_state
{
	texture = <ObjectTexture>;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
};

///////////////////////////////////////////////////////////////////////////////////////////////
// オブジェクト描画（セルフシャドウOFF）

// トゥーンマップのサンプラ。"register(s0)"なのはMMDがs0を使っているから
sampler ToonTexSampler : register(s0);

struct VS_OUTPUT
{
	float4 Pos        : POSITION;    // 射影変換座標
	float2 Tex        : TEXCOORD1;   // テクスチャ
	float3 Normal     : TEXCOORD2;   // 法線
	float3 Eye        : TEXCOORD3;   // カメラとの相対位置
	float4 Color      : COLOR0;      // ディフューズ色
	float3 Specular   : COLOR1;      // スペキュラ色
};

// 頂点シェーダ
VS_OUTPUT Basic_VS(float4 Pos : POSITION, float3 Normal : NORMAL, float2 Tex : TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	// カメラ視点のワールドビュー射影変換
	Out.Pos = mul(Pos, WorldViewProjMatrix);

	// カメラとの相対位置
	Out.Eye = CameraPosition - mul(Pos, WorldMatrix);
	// 頂点法線
	Out.Normal = normalize(mul(Normal, (float3x3)WorldMatrix));

	// ディフューズ色＋アンビエント色 計算
	Out.Color.rgb = saturate(max(0, dot(Out.Normal, -LightDirection)) * DiffuseColor.rgb + AmbientColor);
	Out.Color.a = DiffuseColor.a;

	// テクスチャ座標
	Out.Tex = Tex;

	// スペキュラ色計算
	float3 HalfVector = normalize(normalize(Out.Eye) + -LightDirection);
		Out.Specular = pow(max(0, dot(HalfVector, Out.Normal)), SpecularPower) * SpecularColor;

	return Out;
}

// ピクセルシェーダ
float4 Basic_PS(VS_OUTPUT IN) : COLOR0
{
	float4 Color = IN.Color;
	if (use_texture) {  //※このif文は非効率的
		// テクスチャ適用
		Color *= tex2D(ObjTexSampler, IN.Tex);
	}
	if (use_toon) {  //同上
		// トゥーン適用
		float LightNormal = dot(IN.Normal, -LightDirection);
		Color *= tex2D(ToonTexSampler, float2(0, 0.5 - LightNormal * 0.5));
	}
	// スペキュラ適用
	Color.rgb += IN.Specular;

	return Color;
}

// オブジェクト描画用テクニック
technique MainTec < string MMDPass = "object"; > {
	pass DrawObject
	{
		VertexShader = compile vs_2_0 Basic_VS();
		PixelShader = compile ps_2_0 Basic_PS();
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////
// オブジェクト描画（セルフシャドウON）

// シャドウバッファのサンプラ。"register(s0)"なのはMMDがs0を使っているから
sampler DefSampler : register(s0);

struct BufferShadow_OUTPUT
{
	float4 Pos      : POSITION;     // 射影変換座標
	float4 ZCalcTex : TEXCOORD0;    // Z値
	float2 Tex      : TEXCOORD1;    // テクスチャ
	float3 Normal   : TEXCOORD2;    // 法線
	float3 Eye      : TEXCOORD3;    // カメラとの相対位置
	float4 Color    : COLOR0;       // ディフューズ色
};

// 頂点シェーダ
BufferShadow_OUTPUT BufferShadow_VS(float4 Pos : POSITION, float3 Normal : NORMAL, float2 Tex : TEXCOORD0)
{
	BufferShadow_OUTPUT Out = (BufferShadow_OUTPUT)0;

	// カメラ視点のワールドビュー射影変換
	Out.Pos = mul(Pos, WorldViewProjMatrix);

	// カメラとの相対位置
	Out.Eye = CameraPosition - mul(Pos, WorldMatrix);
	// 頂点法線
	Out.Normal = normalize(mul(Normal, (float3x3)WorldMatrix));
	// ライト視点によるワールドビュー射影変換
	Out.ZCalcTex = mul(Pos, LightWorldViewProjMatrix);

	// ディフューズ色＋アンビエント色 計算
	Out.Color.rgb = saturate(max(0, dot(Out.Normal, -LightDirection)) * DiffuseColor.rgb + AmbientColor);
	Out.Color.a = DiffuseColor.a;

	// テクスチャ座標
	Out.Tex = Tex;

	return Out;
}

// ピクセルシェーダ
float4 BufferShadow_PS(BufferShadow_OUTPUT IN) : COLOR
{
	// スペキュラ色計算
	float3 HalfVector = normalize(normalize(IN.Eye) + -LightDirection);
	float3 Specular = pow(max(0, dot(HalfVector, normalize(IN.Normal))), SpecularPower) * SpecularColor;

	float4 Color = IN.Color;
	float4 ShadowColor = float4(saturate(AmbientColor), Color.a);  // 影の色
	if (use_texture) {
		// テクスチャ適用
		float4 TexColor = tex2D(ObjTexSampler, IN.Tex);
			Color *= TexColor;
		ShadowColor *= TexColor;
	}
	// スペキュラ適用
	Color.rgb += Specular;

	// テクスチャ座標に変換
	IN.ZCalcTex /= IN.ZCalcTex.w;
	float2 TransTexCoord;
	TransTexCoord.x = (1.0f + IN.ZCalcTex.x)*0.5f;
	TransTexCoord.y = (1.0f - IN.ZCalcTex.y)*0.5f;

	if (TransTexCoord.x<0.0f || TransTexCoord.y<0.0f || TransTexCoord.x>1.0f || TransTexCoord.y>1.0f) {
		// シャドウバッファ外
		return Color;
	}
	else {
		float comp;
		if (parthf) {
			// セルフシャドウ mode2
			comp = 1 - saturate(max(IN.ZCalcTex.z - tex2D(DefSampler, TransTexCoord).r, 0.0f)*SKII2*TransTexCoord.y - 0.3f);
		}
		else {
			// セルフシャドウ mode1
			comp = 1 - saturate(max(IN.ZCalcTex.z - tex2D(DefSampler, TransTexCoord).r, 0.0f)*SKII1 - 0.3f);
		}
		if (use_toon) {
			// トゥーン適用
			comp = min(saturate(dot(IN.Normal, -LightDirection)*Toon), comp);
			ShadowColor.rgb *= MaterialToon;
		}
		float4 ans = lerp(ShadowColor, Color, comp);
		if (transp) ans.a = 0.5f;
		return ans;
	}
}

// オブジェクト描画用テクニック
technique MainTecBS  < string MMDPass = "object_ss"; > {
	pass DrawObject {
		VertexShader = compile vs_2_0 BufferShadow_VS();
		PixelShader = compile ps_2_0 BufferShadow_PS();
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////
