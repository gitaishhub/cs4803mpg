float4x4 World;
float4x4 View;
float4x4 Projection;



Texture Diffuse;
Texture Normal;
Texture Specular;
sampler2D DiffuseSampler = sampler_state { texture = <Diffuse>; };
sampler2D NormalSampler = sampler_state { texture = <Normal>; };
sampler2D SpecularSampler = sampler_state { texture = <Specular>; };

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

struct VertexShaderInput
{
    float4 position : POSITION0;
    float2 texCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.

    return float4(1, 0, 0, 1);
    
    
    
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
    
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}