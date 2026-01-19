

//This shader samples the current pixel and surrounding texel to look for region borders, 1 = border, 0 = elsewhere



void RegionEdgeMask_float(Texture2D regionTex, SamplerState regionSampler, float2 UV, float2 texelSize, float threshold, float borderThickness = 1, out float edgeMask) // (texelSize = 1/width, 1/height)
{
    texelSize *= borderThickness;
    
    float3 c0 = regionTex.Sample(regionSampler, UV).rgb;

    float3 cx1 = regionTex.Sample(regionSampler, UV + float2(texelSize.x, 0)).rgb;
    float3 cx2 = regionTex.Sample(regionSampler, UV - float2(texelSize.x, 0)).rgb;
    float3 cy1 = regionTex.Sample(regionSampler, UV + float2(0, texelSize.y)).rgb;
    float3 cy2 = regionTex.Sample(regionSampler, UV - float2(0, texelSize.y)).rgb;

    float d = max( max(length(c0 - cx1) , length(c0 - cx2)), max(length(c0 - cy1) , length(c0 - cy2)) );

    edgeMask = step(threshold, d);
}