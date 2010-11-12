//
// ECE4893 Multicore and GPU programming for Video Games
//
// Author: Hsien-Hsin S. Lee
// Email:  leehs@gatech.edu
//
// Coments:
// The way I implemented this program is to minimize the 
// usage of registers so it will get compiled under
// shader model 2.0. So you will find a lot of room for 
// improvement in terms of code quality when you port the 
// same algorithm to later shader model
//

struct lightProperty {
	float4 position;
	float4 diffuseLight;
	float4 ambientLight;
	float4 specLight;
	float  shininess;
	int    on;
	float4 attenuation;
	int    is_pointLight;
	float4 lightDir;
};

struct materialProperty {
	float4 a_material;
	float4 d_material;
	float4 s_material;
};
	
uniform extern int numLights;
uniform extern lightProperty light[4];
uniform extern materialProperty material;

uniform extern float4x4 gWVP;
uniform extern float4x4 gWorld;

uniform extern float3 eyePosition;

uniform extern float morphrate;
uniform extern texture map;
uniform extern texture map2;

struct LightComponents {
	float4 ambient;
	float4 diffuse;
	float4 spec;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
    float4 ColorDiff : TEXCOORD1;
};

struct PixelShaderOutput
{
    float4 Color : COLOR;
};

struct PhongVSOutput
{
	float4 Position    : POSITION;
    float2 TexCoord    : TEXCOORD0;
    float3 NormalT     : TEXCOORD1;
};



sampler textureBase = sampler_state
{
  Texture = <map>;
  mipfilter = LINEAR;
};

sampler textureSpecial = sampler_state
{
  Texture = <map2>;
  mipfilter = LINEAR;
};



VertexShaderOutput GouroudVS(VertexShaderInput input)
{
    VertexShaderOutput output;
	float  intensity, spec_intensity;
	int i;
	LightComponents liteComponents[4]; // my shader constraint, won't compile if too big

    output.Color = float4(0.0f, 0.0f, 0.0f, 0.0f); 
    output.ColorDiff = float4(0.0f, 0.0f, 0.0f, 0.0f); 
    
    float3 TransP;
    float3 TransN;
	float3 H, L, V;
	float dist, atten;
    
    TransP = mul(float4(input.Position, 1.0f), gWorld).xyz;
    TransN = mul(float4(input.Normal, 0.0f), gWorld).xyz;    
    
    V = normalize(eyePosition - TransP);   
    
    for (i=0; i<numLights; i++) 
    {
		liteComponents[i].ambient = 0.0f;
		liteComponents[i].diffuse = 0.0f;
		liteComponents[i].spec = 0.0f;
		atten = 1.0f;
		
		if (light[i].on != 0) 
		{
			liteComponents[i].ambient = light[i].ambientLight * material.a_material;
			
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
    
    
	// transformation
    output.Position = mul(float4(input.Position, 1.0f), gWVP);
    output.TexCoord = input.TexCoord;
    
    return output;
}




float4 GouroudPlainPS(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput pout;
	
	pout.Color = input.Color;
	
    return pout.Color;
}


float4 GouroudPS(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput pout;
	
	float4 temp = tex2D(textureBase, input.TexCoord);
	
	pout.Color = temp*input.Color;
	
    return pout.Color;
}


float4 MorphPS(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput pout;
	
	float4 temp  = tex2D(textureBase,  input.TexCoord);
	float4 temp2 = tex2D(textureSpecial, input.TexCoord);
	
	
	if (morphrate >= 0.0f)
	{
		pout.Color = lerp(temp, temp2, morphrate)*input.Color;
	}
	else
	{  // control timing...
		pout.Color = lerp(temp, temp2, 0.0f)*input.Color;
	}
		
    return pout.Color;
}

float4 LightMapPS(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput pout;
	
	float4 tempS  = tex2D(textureSpecial, input.TexCoord);
	float4 tempB  = tex2D(textureBase, input.TexCoord);
	float4 temp1;
	
	temp1 = lerp(tempB, input.ColorDiff, 0.5f); // I only used Diffuse color
	pout.Color = (temp1*tempS) * 2.8;  // close to D3DTOP_MODULATE2X
	
    return pout.Color;
}

PhongVSOutput PhongVS(VertexShaderInput input)
{
	PhongVSOutput pout;
	
    
	pout.Position = mul(float4(input.Position, 1.0f), gWVP);
	pout.TexCoord = input.TexCoord;
	pout.NormalT  = mul(float4(input.Normal, 0.0f), gWorld).xyz;    
	
    return pout;
}

technique myPlainTech
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 GouroudVS();
        PixelShader =  compile ps_2_0 GouroudPlainPS();
        FillMode = Solid;
    }
}

technique myPlainWireTech
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 GouroudVS();
        PixelShader =  compile ps_2_0 GouroudPlainPS();
        FillMode = WireFrame;
    }
}


technique myTech
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 GouroudVS();
        PixelShader =  compile ps_2_0 GouroudPS();
         FillMode = Solid;
    }

}

technique MorphTech
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 GouroudVS();
        PixelShader =  compile ps_2_0 MorphPS();
        FillMode = Solid;
    }

}

technique LightMapTech
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 GouroudVS();
        PixelShader =  compile ps_2_0 LightMapPS();
        FillMode = Solid;
    }

}
