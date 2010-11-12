/*
 * ECE4893
 * Multicore and GPU Programming for Video Games
 * 
 * Author: Hsien-Hsin Sean Lee
 * 
 * School of Electrical and Computer Engineering
 * Georgia Tech
 * 
 * leehs@gatech.edu
 * 
 * */
uniform extern float4x4 gWVP;
uniform extern float4x4 gWorld;

//uniform extern float3 eyePosition;

uniform extern texture wall;



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
};

struct PixelShaderOutput
{
    float4 Color : COLOR;
};

sampler textureWall = sampler_state
{
  Texture = <wall>;
  mipfilter = LINEAR;
};




VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	// transformation
    output.Position = mul(float4(input.Position, 1.0f), gWVP);
    output.TexCoord = input.TexCoord;
    
    return output;
}


float4 WallPS(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput pout;
	
	float4 temp = tex2D(textureWall, input.TexCoord);
	
	pout.Color = temp;
	
    return pout.Color;
}

technique myTech
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader  =  compile ps_2_0 WallPS();
    }

}
