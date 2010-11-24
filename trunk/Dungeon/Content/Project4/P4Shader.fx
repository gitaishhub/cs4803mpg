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

//General variables.
uniform extern float4x4 World;
uniform extern float4x4 View;
uniform extern float4x4 Projection;
uniform extern float3 Viewpoint;
uniform extern float4x4 VPInverse;
uniform extern float4x4 previousVPInverse;
uniform extern int numSamples;

//Rendering variables.
uniform extern Light Lights[4];
uniform extern int NumLights;
uniform extern MaterialProperty Material;

uniform extern Texture Diffuse;
uniform extern Texture Normal;
uniform extern Texture Specular;
uniform sampler2D DiffuseSampler = sampler_state { texture = <Diffuse>; mipfilter = LINEAR; };
uniform sampler2D NormalSampler = sampler_state { texture = <Normal>; mipfilter = LINEAR; };
uniform sampler2D SpecularSampler = sampler_state { texture = <Specular>; mipfilter = LINEAR; };

//Depth variables.
uniform extern texture ShadowCube;
uniform samplerCUBE ShadowCubeSampler = sampler_state { texture = <ShadowCube>; };

//Post-processing variables.
uniform extern Texture DepthTexture;
uniform extern Texture ScreenTexture;
uniform sampler2D ScreenSampler = sampler_state { texture = <ScreenTexture>; mipfilter = linear; };
uniform sampler2D DepthSampler = sampler_state { texture = <DepthTexture>; mipfilter = linear; };

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 Position			: POSITION0;
    float3 WorldPosition    : TEXCOORD1;
    float2 TexCoord			: TEXCOORD0;
};

VertexShaderOutput VertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.WorldPosition = mul(input.Position, World);
    output.TexCoord = input.TexCoord;
    
    return output;
}

float4 BumpFragmentShader(VertexShaderOutput input) : COLOR0
{
	//Lookup normal from texture.   
    float3 normal = tex2D(NormalSampler, input.TexCoord);
    normal = (normal - 0.5) / 2;
    normal = mul(float4(normal, 0), World);
    
    float4 lightColor = 0;
    float4 ambient, diffuse, specular;
    float3 V = normalize(Viewpoint - input.WorldPosition);
    float3 L, H;
    float attenuation, dist, lightDepth, A, D, S;
    
    for (int i = 0; i < NumLights; i++) {
		ambient = 0.0f;
		diffuse = 0.0f;
		specular = 0.0f;
		attenuation = 1.0f;
		
		//Get vector to light source.
		L =  Lights[i].Position.xyz - input.WorldPosition;
		//Calculate distance to light.
		dist  = distance(Lights[i].Position, input.WorldPosition);		
		//Normalize L (we already have dist, so I'll just divide).
		L = L / dist;
		//Lookup depth value in shadow cube.
		lightDepth = 150;//texCUBE(ShadowCubeSampler, -L);
		
		//If pixel is in shadow, do nothing.  Else color the pixel for this light.
		if (Lights[i].On && dist <= lightDepth ) {
			//Calculate light ray and half-angle.
			/*if (Lights[i].IsPointLight) {
				L =  Lights[i].Position - input.WorldPosition;
			} else {
				L = -Lights[i].Direction;
			}*/
			
			L = normalize(L);
			H = normalize(L+V);
			 
			//Calculate diffuse, specular, and attenuation coefficients.								
			D =		max(dot(normal, L), 0);
			//S = pow(max(dot(normal, H), 0), Lights[i].Shininess);
			//A = Lights[i].Attenuation.x +  Lights[i].Attenuation.y*dist + Lights[i].Attenuation.y*dist*dist;
			
			
			//ambient = Material.Ambient * Lights[i].AmbientLight;
			diffuse = /*Material.Diffuse */ Lights[i].DiffuseLight * D;
			//specular = Material.Specular * Lights[i].SpecLight * S;		
		}
		
		lightColor = lightColor + diffuse; //ambient + (diffuse + specular); // A;
    }
    
    float4 color = tex2D(DiffuseSampler, input.TexCoord);
    return color * lightColor;
}

//Depth Mapping.
struct DepthVertexShaderOutput {
	float4 Position	: POSITION0;
    float4 DepthRay	: TEXCOORD0;
};

DepthVertexShaderOutput DepthVertexShader(VertexShaderInput input) {
	DepthVertexShaderOutput output;
	
	output.Position = mul(mul(mul(input.Position, World), View), Projection);
	output.DepthRay = output.Position;
	
	return output;
}

float4 DepthFragmentShader(DepthVertexShaderOutput input) : COLOR0 {
	return input.DepthRay.z / input.DepthRay.w;
}

//Motion Blurring.
float4 MotionBlurPixel(float2 TexCoord : TEXCOORD0) : COLOR0
{
	//Get depth buffer value at pixel.
	/*float4 depth = tex2D(DepthSampler, TexCoord);
	float zOverW = depth.z / depth.w;
	//Calculate viewport position at pixel -1 to 1.
	float4 view = float4(TexCoord.x * 2 - 1, (1 - TexCoord.y) * 2 - 1, zOverW, 1);
	//Tranform by view-proj inverse.
	float4 trans = mul(view, VPInverse);
	//Divide by w to get world position.
	float4 worldPos = trans / trans.w;
	
	float4 currentPos = view;
	float4 previousPos = mul(worldPos, previousVPInverse);
	previousPos /= previousPos.w;
	float2 velocity = (currentPos - previousPos)/2.0f;
	
	//get initial color of pixel
	float4 color = float4(0, 0, 0, 0);
	for(int i = 0; i < numSamples; i++)
	{
		color += tex2D(ScreenSampler, TexCoord);
		TexCoord += velocity;
	}*/
	return tex2D(ScreenSampler, TexCoord);	
}

technique BumpMapped
{
    pass Bump
    {
        VertexShader = compile vs_3_0 VertexShader();
        PixelShader = compile ps_3_0 BumpFragmentShader();
    }
}

technique DepthMapped {
	pass Depth {
		VertexShader = compile vs_3_0 DepthVertexShader();
        PixelShader = compile ps_3_0 DepthFragmentShader();
	}
}

technique MotionBlurred
{
    pass Motion
    {
        PixelShader = compile ps_3_0 MotionBlurPixel();
    }
}