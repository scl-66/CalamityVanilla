sampler uImage0 : register(s0);

float4 uPanelDimensions; // x, y, width, height in screen pixels
float4 uGlowColor;       
float uTime;

#define POSITION uPanelDimensions.xy
#define SIZE uPanelDimensions.zw

float4 main(float2 coords : SV_POSITION, float2 tex_coords : TEXCOORD0, float4 vColor : COLOR0) : COLOR0 {
    float4 baseColor = tex2D(uImage0, tex_coords) * vColor;
    
    if (baseColor.a <= 0) return baseColor;

    float localX = coords.x - POSITION.x;
    float progress = saturate(localX / SIZE.x);
    
    float fade = saturate(1.0 - (progress * 2.0));
    fade = pow(fade, 1.5);
    
    float pulse = (sin(uTime * 5.0) * 0.15 + 0.35);
    float3 finalRgb = lerp(baseColor.rgb, uGlowColor.rgb, fade * pulse);
    
    return float4(finalRgb, baseColor.a);
}

#ifdef FX
technique Technique1 {
    pass Fade {
        PixelShader = compile ps_3_0 main();
    }
}
#endif