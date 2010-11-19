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

uniform extern Texture Diffuse;
uniform extern Texture Normal;
uniform extern Texture Specular;
uniform sampler2D DiffuseSampler = sampler_state { texture = <Diffuse>; };
uniform sampler2D NormalSampler = sampler_state { texture = <Normal>; };
uniform sampler2D SpecularSampler = sampler_state { texture = <Specular>; };

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

struct LightOutput {
	float4 Ambient;
	float4 Diffuse;
	float4 Specular;
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
    
    float3 normal = tex2D(NormalSampler, input.TexCoord);
    normal = (normal - 0.5) / 2;
    
    LightOutput lightOutputs[4];
    
    for (int i = 0; i < NumLights; i++) {
		lightOutputs[i].ambient = 0.0f;
		lightOutputs[i].diffuse = 0.0f;
		lightOutputs[i].spec = 0.0f;
		attenuation = 1.0f;
		
		if (light[i].on != 0) {
			lightOutputs[i].ambient = light[i].ambientLight * material.a_material;
			
			// trick to not use if-then to generate correct code on SM2.0
			L =	normalize((0 - light[i].lightDir.xyz) * light[i].is_pointLight +
							(light[i].position.xyz - TransP) * (1 - light[i].is_pointLight));
			 

								
			intensity = max(dot(TransN, L), 0);
			liteComponents[i].diffuse = material.d_material * light[i].diffuseLight * intensity;
			
			// specular effect for just the light specified specular is on
			H = normalize(L+V);
			spec_intensity = pow(max(dot(TransN, H), 0), light[i].shininess);
			liteComponents[i].spec = material.s_material * light[i].specLight * spec_intensity;		
			
			dist  = distance(light[i].position.xyz, TransP);
			atten = light[i].attenuation.x +  light[i].attenuation.y*dist + light[i].attenuation.y*dist*dist; 			
		}
		
		output.Color = output.Color + liteComponents[i].ambient + ((liteComponents[i].diffuse + liteComponents[i].spec) / atten);
		output.ColorDiff = output.ColorDiff + (liteComponents[i].diffuse / atten);
    }
    
    float4 color = tex2D(DiffuseSampler, input.TexCoord);
    return color;
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