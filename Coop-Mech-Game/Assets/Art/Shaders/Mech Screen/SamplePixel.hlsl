

//This shader samples the current pixel and surrounding texel to look for region borders, 1 = border, 0 = elsewhere



void SamplePixel(Texture2D regionTex, SamplerState regionSampler, float2 UV, float2 texelSize, float threshold, float borderThickness, out float edgeMask) // (texelSize = 1/width, 1/height)
{
    texelSize *= borderThickness;
    
    //pixel from UV coord
    float3 c0 = regionTex.Sample(regionSampler, UV).rgb;

    
    //up/down/left/right
    float3 cx1 = regionTex.Sample(regionSampler, UV + float2(texelSize.x, 0)).rgb;
    float3 cx2 = regionTex.Sample(regionSampler, UV + float2(-texelSize.x, 0)).rgb;
    float3 cy1 = regionTex.Sample(regionSampler, UV + float2(0, texelSize.y)).rgb;
    float3 cy2 = regionTex.Sample(regionSampler, UV + float2(0, -texelSize.y)).rgb;

    //diagonals
    float3 cUR = regionTex.Sample(regionSampler, UV + float2(texelSize.x, texelSize.y)).rgb;
    float3 cUL = regionTex.Sample(regionSampler, UV + float2(-texelSize.x, texelSize.y)).rgb;
    float3 cDR = regionTex.Sample(regionSampler, UV + float2(texelSize.x, -texelSize.y)).rgb;
    float3 cDL = regionTex.Sample(regionSampler, UV + float2(-texelSize.x, -texelSize.y)).rgb;
    
    float dCard = max( max(length(c0 - cx1) , length(c0 - cx2)), max(length(c0 - cy1) , length(c0 - cy2)) );
    float dDiag = max(max(length(c0 - cUR), length(c0 - cUL)), max(length(c0 - cDR), length(c0 - cDL))) * 0.70710678; // 1 / sqrt(2)
    
    float d = max(dCard, dDiag);

    edgeMask = smoothstep(threshold, threshold + 0.02, d);
}