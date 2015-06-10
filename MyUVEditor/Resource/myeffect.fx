matrix World;
matrix ViewProjection;
float3 LightDirection;
Texture2D diffuseTexture;

SamplerState mySampler{

};

struct PmxVertexStruct{
	float4 Position :SV_Position;
	float2 Tex : TEXCOORD;
};

PmxVertexStruct MyVertexShader(PmxVertexStruct input)
{
	PmxVertexStruct output = input;
	return output;
}

float4 MyPixelShader(PmxVertexStruct input) : SV_Target
{
	return diffuseTexture.Sample(mySampler, input.Tex);
}

technique10 MyTechnique
{
	pass MyPass
	{
		SetVertexShader(CompileShader(vs_5_0, MyVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader()));
	}
}