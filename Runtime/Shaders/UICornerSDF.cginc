// UICorner SDF - GPU-accelerated rounded and chamfered corners for Unity UI
// Based on Inigo Quilez's quadrant-selection approach for optimal performance

float AntialiasedCutoff(float distance)
{
    float distanceChange = fwidth(distance) * 0.5;
    return smoothstep(distanceChange, -distanceChange, distance);
}

// Per-corner rounded/chamfered box SDF
// p: Point to evaluate (centered coordinates)
// b: Half-size of box
// r: Corner radii (x=topLeft, y=topRight, z=bottomRight, w=bottomLeft)
// cornerTypes: Corner styles (0=rounded, 1=chamfered for each corner)
float mixedBoxSDF(float2 p, float2 b, float4 r, float4 cornerTypes)
{
    const float SQRT2 = 1.414213562;

    // Quadrant-based corner selection (IQ's approach)
    // Maps to correct corner based on point position
    r.xy = (p.x > 0.0) ? r.yz : r.xw;
    r.x = (p.y > 0.0) ? r.x : r.y;

    cornerTypes.xy = (p.x > 0.0) ? cornerTypes.yz : cornerTypes.xw;
    cornerTypes.x = (p.y > 0.0) ? cornerTypes.x : cornerTypes.y;

    // Work in first quadrant
    float2 q = abs(p);

    // Rounded corner (circular arc)
    float2 qRounded = q - b + r.x;
    float roundedDist = min(max(qRounded.x, qRounded.y), 0.0) + length(max(qRounded, 0.0)) - r.x;

    // Chamfered corner (45-degree diagonal cut)
    float2 cornerOffset = q - b;
    float chamferLineDist = (cornerOffset.x + cornerOffset.y + r.x) / SQRT2;
    float edgeDist = max(cornerOffset.x, cornerOffset.y);
    float chamferDist = max(edgeDist, chamferLineDist);

    // Blend between styles
    return lerp(roundedDist, chamferDist, cornerTypes.x);
}

float CalcAlphaForUICorner(float2 samplePosition, float2 halfSize, float4 rect2props, float4 r, float4 cornerTypes)
{
    float2 p = (samplePosition - 0.5) * halfSize * 2.0;
    float distance = mixedBoxSDF(p, halfSize, r, cornerTypes);
    return AntialiasedCutoff(distance);
}
