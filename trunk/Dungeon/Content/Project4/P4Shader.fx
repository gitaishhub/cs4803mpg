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
	float4x4 World;
	float4x4 View;
	float4x4 Projection;
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
uniform extern texture ShadowMap;
uniform sampler2D ShadowSampler = sampler_state { texture = <ShadowMap>; };

//Post-processing variables.
uniform extern Texture ScreenTexture;
uniform sampler2D ScreenSampler = sampler_state { texture = <ScreenTexture>; mipfilter = linear; };

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 Position	: POSITION0;
    float2 TexCoord	: TEXCOORD0;
    float4 LightPos	: TEXCOORD1;
    float4 WorldPos	: TEXCOORD2;
};

VertexShaderOutput VertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.WorldPos = mul(input.Position, World);
    output.LightPos = mul(mul(mul(input.Position, Lights[0].World), Lights[0].View), Lights[0].Projection);
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
    float3 V = normalize(Viewpoint - input.WorldPos);
    float3 L, H;
    float2 lightTex;
    float attenuation, dist, lightDepth, pixelDepth, A, D, S;
    
    for (int i = 0; i < NumLights; i++) {
		ambient = 0.0f;
		diffuse = 0.0f;
		specular = 0.0f;
		attenuation = 1.0f;
		
		//Get vector to light source.
		L =  Lights[i].Position.xyz - input.WorldPos;		
		//Normalize L.
		L = normalize(L);
		
		//Lookup depth value in shadow cube.
		lightTex.x =  input.LightPos.x / input.LightPos.w / 2 + 0.5;
		lightTex.y = -input.LightPos.y / input.LightPos.w / 2 + 0.5;
		//lightTex = saturate(lightTex);
		
		
		lightDepth = tex2D(ShadowSampler, lightTex).r;
		pixelDepth = input.LightPos.z / input.LightPos.w;
		
		//If pixel is in shadow, do nothing.  Else color the pixel for this light.
		if (Lights[i].On && pixelDepth - 0.01 <= lightDepth) {
			//Calculate light ray and half-angle.
			if (Lights[i].IsPointLight) {
				L =  Lights[i].Position - input.WorldPos;
				dist = length(L);
			} else {
				L = -Lights[i].Direction;
				dist = 1;
			}
			
			L = normalize(L);
			H = normalize(L+V);
			
			
			//Calculate diffuse, specular, and attenuation coefficients.								
			D =		max(dot(normal, L), 0);
			S = pow(max(dot(normal, H), 0), Lights[i].Shininess);
			A = Lights[i].Attenuation.x +  Lights[i].Attenuation.y*dist + Lights[i].Attenuation.y*dist*dist;
			
			
			ambient = Material.Ambient * Lights[i].AmbientLight;
			diffuse = /*Material.Diffuse */ Lights[i].DiffuseLight * D;
			specular = Material.Specular * Lights[i].SpecLight * S;		
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

//Blurring.
float4 BlurPixel(float2 TexCoord : TEXCOORD0) : COLOR0
{
	/*//Get depth buffer value at pixel.
	float4 depth = tex2D(DepthSampler, TexCoord);
	float zOverW = depth.z / depth.w;
	//Calculate viewport position at pixel -1 to 1.
	float4 view = float4(TexCoord.x * 2 - 1, (1 - TexCoord.y) * 2 - 1, zOverW, 1);
	//Tranform by view-proj inverse.
	float4 trans = mul(view, VPInverse);
	//Divide by w to get world position.
	float4 worldPos = trans / trans.w;
	
	float4 currentPos = view;
	float4 previousPos = mul(worldPos, PreviousVPInverse);
	previousPos /= previousPos.w;
	float2 velocity = (currentPos - previousPos)/2.0f;
	
	//get initial color of pixel
	float4 color = float4(0, 0, 0, 0);
	for(int i = 0; i < NumSamples; i++)
	{
		color += tex2D(ScreenSampler, TexCoord);
		TexCoord += velocity;
	}
	return color / NumSamples;*/
	
	float blur = 0.008;
	
	float2 up    = float2(TexCoord.x, TexCoord.y + blur);
	float2 right = float2(TexCoord.x + blur, TexCoord.y);
	float2 down  = float2(TexCoord.x, TexCoord.y - blur);
	float2 left  = float2(TexCoord.x - blur, TexCoord.y);
	
	float4 color = 5 * tex2D(ScreenSampler, TexCoord);
	color += tex2D(ScreenSampler, up);
	color += tex2D(ScreenSampler, right);
	color += tex2D(ScreenSampler, down);
	color += tex2D(ScreenSampler, left);
	
	return color / 9;
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

technique Blurred
{
    pass Blur
    {
        PixelShader = compile ps_3_0 BlurPixel();
    }
}