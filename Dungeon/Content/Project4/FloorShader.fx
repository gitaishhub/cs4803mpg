struct Light {
	float4 Position;
	float4 Direction;
	float4 AmbientLight;
	float4 DiffuseLight;
	float4 SpecLight;
	float  Shininess;
	int    On;
	int    IsPointLight;
	float4 Attenuation;
};

struct MaterialProperty {
	float4 Ambient;
	float4 Diffuse;
	float4 Specular;
};

uniform extern float4x4 World;
uniform extern float4x4 WVP;
uniform extern float3 Viewpoint;
uniform extern Light Lights[4];
uniform extern int NumLights;
uniform extern MaterialProperty Material;

uniform extern Texture Diffuse;
uniform extern Texture Normal;
uniform extern Texture Specular;
uniform sampler2D DiffuseSampler = sampler_state { texture = <Diffuse>; mipfilter = LINEAR; };
uniform sampler2D NormalSampler = sampler_state { texture = <Normal>; mipfilter = LINEAR; };
uniform sampler2D SpecularSampler = sampler_state { texture = <Specular>; mipfilter = LINEAR; };

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
	//Lookup normal from texture.   
    float3 normal = tex2D(NormalSampler, input.TexCoord);
    normal = (normal - 0.5) / 2;
    
    
    float4 lightColor = 0;
    float4 ambient, diffuse, specular;
    float3 V = normalize(Viewpoint - input.WorldPosition.xyz);
    float3 L, H;
    float attenuation, dist, A, D, S;
    
    for (int i = 0; i < NumLights; i++) {
		ambient = 0.0f;
		diffuse = 0.0f;
		specular = 0.0f;
		attenuation = 1.0f;
		
		if (Lights[i].On) {
			//Calculate light ray and half-angle.
			if (Lights[i].IsPointLight) {
				L =  Lights[i].Position.xyz - input.WorldPosition.xyz;
			} else {
				L = -Lights[i].Direction.xyz;
			}
			H = normalize(L+V);
			 
			//Calculate distance to light.
			dist  = distance(Lights[i].Position.xyz, input.WorldPosition.xyz);
			
			//Calculate diffuse, specular, and attenuation coefficients.								
			D =		max(dot(normal, L), 0);
			S = pow(max(dot(normal, H), 0), Lights[i].Shininess);
			A = Lights[i].Attenuation.x +  Lights[i].Attenuation.y*dist + Lights[i].Attenuation.y*dist*dist;
			
			
			ambient = Lights[i].AmbientLight * Material.Ambient;
			diffuse = Material.Diffuse * Lights[i].DiffuseLight * D;
			specular = Material.Specular * Lights[i].SpecLight * S;		
		}
		
		lightColor = lightColor + ambient + (diffuse + specular) / A;
    }
    
    float4 color = tex2D(DiffuseSampler, input.TexCoord);
    return color * lightColor;
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