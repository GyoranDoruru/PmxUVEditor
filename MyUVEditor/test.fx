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

float3 UV_VS(float2 Tex : TEXCOORD0) : POSITION
{
	return float3(Tex.X, Tex.Y, 0);
}

float4 UV_VS() : COLOR0
{
	return float4(1, 1, 1, 1);
}

technique UVTec{
	pass DrawUV{
		VertexShader = compile vs_2_0 UV_VS();
		PixelShader = compile ps_2_0 UV_PS();
	}
}