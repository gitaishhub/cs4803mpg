struct Light {
	float4 Position;
	float4 Direction;
	int    Is_pointLight;
	float4 AmbientLight;
	float4 DiffuseLight;
	float4 SpecLight;
	float  Shininess;
	int    On;
	float4 Attenuation;
	

};

uniform extern float4x4 World;
uniform extern float4x4 WVP;
uniform extern float3 Viewpoint;
uniform extern Light Lights[4];
uniform extern int NumLights;

Texture Diffuse;
Texture Normal;
Texture Specular;
sampler2D DiffuseSampler = sampler_state { texture = <Diffuse>; };
sampler2D NormalSampler = sampler_state { texture = <Normal>; };
sampler2D SpecularSampler = sampler_state { texture = <Specular>; };

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 Position			: POSITION0;
    float3 WorldPosition    : POSITION1;
    float2 TexCoord			: TEXCOORD0;

};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Position = mul(input.Position, WVP);
    output.WorldPosition = mul(input.Position, World);
    output.TexCoord = input.TexCoord;
    
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Fetch and expand range-compressed normal
    //float3 normalTex = tex2D(normalMap, normalMapTexCoord).xyz;
    //float3 normal = expand(normalTex);
    
    // Fetch and expand normalized light vector
    //float3 normLightDirTex = texCUBE(normalizeCube, lightDirection).xyz;
    //float3 normLightDir = expand(normLightDirTex);
    
    // Fetch and expand normalized half-angle vector
    //float3 normHalfAngleTex = texCUBE(normalizeCube2, halfAngle).xyz;
    //float3 normHalfAngle = expand(normHalfAngleTex);
 
    // Compute diffuse and specular lighting dot products
    //float diffuse = saturate(dot(normal, normLightDir));
    //float specular = saturate(dot(normal, normHalfAngle));
    
    // Successive multiplies to raise specular to 8th power
    //float specular2 = specular*specular;
    //float specular4 = specular2*specular2;
    //float specular8 = specular4*specular4;

    //color = LMd*(ambient+diffuse) + LMs*specular8;
    return tex2D(DiffuseSampler, input.TexCoord);
}

technique BumpMapped
{
    pass Bump
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}