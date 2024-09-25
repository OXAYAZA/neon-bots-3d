#define PI 3.14159265358979323846
#define TWO_PI 6.28318530718
#define VALUES_N 10

#pragma multi_compile_local __ LAYER_0
#pragma multi_compile_local __ LAYER_1
#pragma multi_compile_local __ LAYER_2
#pragma multi_compile_local __ LAYER_3
#pragma multi_compile_local __ LAYER_4
#pragma multi_compile_local __ LAYER_5
#pragma multi_compile_local __ LAYER_6
#pragma multi_compile_local __ LAYER_7
#pragma multi_compile_local __ LAYER_8
#pragma multi_compile_local __ LAYER_9

struct appdata
{
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
    float4 uv1 : TEXCOORD1;
    float4 color : COLOR;
};

struct v2f
{
    float4 position : SV_POSITION;
    float4 worldPosition : TEXCOORD1;
    float2 uv : TEXCOORD0;
    float4 color : COLOR;
};

static const half4 Transp = half4(0.0, 0.0, 0.0, 0.0);
static const half4 White = half4(1.0, 1.0, 1.0, 1.0);

static half2 FullSize;
static half2 HalfSize;
static half2 ExpCoeff;
static v2f Inp;
static half2 Pos;
static half FigD;
static half4 FigMask;

half4 _CornersRadius;
half2 _Size;
half2 _Expansion;

half _LayerValues0[VALUES_N];
half _LayerValues1[VALUES_N];
half _LayerValues2[VALUES_N];
half _LayerValues3[VALUES_N];
half _LayerValues4[VALUES_N];
half _LayerValues5[VALUES_N];
half _LayerValues6[VALUES_N];
half _LayerValues7[VALUES_N];
half _LayerValues8[VALUES_N];
half _LayerValues9[VALUES_N];

sampler2D _LayerTexture0;
sampler2D _LayerTexture1;
sampler2D _LayerTexture2;
sampler2D _LayerTexture3;
sampler2D _LayerTexture4;
sampler2D _LayerTexture5;
sampler2D _LayerTexture6;
sampler2D _LayerTexture7;
sampler2D _LayerTexture8;
sampler2D _LayerTexture9;

float rotToRad(float rotation)
{
    return rotation * PI * 2 * -1;
}

// TODO: Use matrices for rotation?
// https://en.wikipedia.org/wiki/Rotation_matrix
// https://gist.github.com/yiwenl/3f804e80d0930e34a0b33359259b556c
float2 rotate(float2 pos, float rotation)
{
    float angle = rotToRad(rotation);
    float sine, cosine;
    sincos(angle, sine, cosine);
    return float2(cosine * pos.x + sine * pos.y, cosine * pos.y - sine * pos.x);
}

float Rect(float2 pos, float2 center, float2 halfSize)
{
    return length(max(abs(pos - center) - halfSize, 0));
}

// radius.x - top-right
// radius.y - bottom-right
// radius.z - top-left
// radius.w - bottom-left
float sdfRoundBox(float2 p, float2 halfSize, float4 radius)
{
    radius.xy = p.x > 0.0 ? radius.xy : radius.zw;
    radius.x = p.y > 0.0 ? radius.x : radius.y;
    float2 q = abs(p) - halfSize + radius.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - radius.x;
}

float4 RadialGradient(float2 pos, sampler2D tex, float2 center, float r1, float r2, float angle)
{
    float2 uv1 = pos;
    float s = sin(2 * PI * -angle / 360);
    float c = cos(2 * PI * -angle / 360);
    float2x2 rotationMatrix = float2x2(c, -s, s, c);
    rotationMatrix *= 0.5;
    rotationMatrix += 0.5;
    rotationMatrix = rotationMatrix * 2 - 1;
    uv1.xy = mul(uv1.xy - center.xy, rotationMatrix);

    float x =  uv1.x;
    float y =  uv1.y;
    float or1 = r1 / 2;
    float or2 = r2 / 2;
    float2 uv2 = sqrt(x * x / or1 + y * y / or2);
    return tex2D(tex, uv2);
}

float4 DiamondGradient(float2 pos, sampler2D tex, float2 center, float r1, float r2, float angle)
{
    float2 uv1 = pos;
    float s = sin(2 * PI * -angle / 360);
    float c = cos(2 * PI * -angle / 360);
    float2x2 rotationMatrix = float2x2(c, -s, s, c);
    rotationMatrix *= 0.5;
    rotationMatrix += 0.5;
    rotationMatrix = rotationMatrix * 2 - 1;
    uv1.xy = mul(uv1.xy - center.xy, rotationMatrix);

    float x =  uv1.x;
    float y =  uv1.y;
    float or1 = r1 / 2;
    float or2 = r2 / 2;
    float2 uv2 = abs(x) / or1 + abs(y) / or2;
    return tex2D(tex, uv2);
}

float4 SomeBlur(float2 pos, sampler2D tex, float directions, float quality, float2 radius)
{
    // Pixel colour
    float4 Color = tex2D(tex, pos);
    
    // Blur calculations
    for(float d = 0.0; d < TWO_PI; d += TWO_PI / directions)
    {
        for(float i = 1.0 / quality; i <= 1.001; i += 1.0 / quality)
        {
            Color += tex2D(tex, pos + float2(cos(d), sin(d)) * radius * i);
        }
    }

    // Output to screen
    Color /= quality * directions + 1.0;
    return Color;
}

half4 LayerSolidColor(half lv[VALUES_N], sampler2D lt)
{
    return lerp(Transp, half4(lv[1], lv[2], lv[3], lv[4]), FigMask);
}

half4 LayerGradientColor(half lv[VALUES_N], sampler2D lt)
{
    half4 result = Transp;
    half type = lv[1];
    half2 position = half2(lv[2], lv[3]);
    half2 size = half2(lv[4], lv[5]);
    half angle = lv[6];

    if(type == 0.0)
    {
        result = tex2D(lt, rotate((Inp.uv - 0.5) / ExpCoeff, angle) + 0.5);
    }
    else if(type == 1.0)
    {
        result = RadialGradient((Inp.uv - 0.5) / ExpCoeff, lt, position - 0.5, size.x, size.y, 0.0);
    }
    else if(type == 2.0)
    {
        result = DiamondGradient((Inp.uv - 0.5) / ExpCoeff, lt, position - 0.5, size.x, size.y, 0.0);
    }

    return lerp(Transp, result, FigMask);
}

half4 LayerImage(half lv[VALUES_N], sampler2D lt)
{
    half4 result = Transp;
    half2 imgSize = half2(lv[1], lv[2]);
    half2 position = half2(lv[3], lv[4]);
    half blurRadius = lv[5];
    half blurQuality = lv[6];
    half blurDirections = lv[7];
    half2 imgPos = Pos + imgSize * position - FullSize * (position - 0.5);
    half imgRectD = Rect(Pos, FullSize * (position - 0.5) - imgSize * (position - 0.5), imgSize * 0.5);
    half4 imgMask = lerp(Transp, White, imgRectD <= 0);

    if(blurRadius == 0.0)
    {
        half4 img = tex2D(lt, imgPos / imgSize);
        result = lerp(Transp, img, imgMask);
    }
    else
    {
        half4 img = SomeBlur(imgPos / imgSize, lt, blurDirections, blurQuality, blurRadius);
        result = lerp(Transp, img, imgMask);
    }

    return lerp(Transp, result, FigMask);
}

half4 LayerBorder(half lv[VALUES_N], sampler2D lt)
{
    half4 result = Transp;
    half width = lv[1];
    half4 borderColor = half4(lv[2], lv[3], lv[4], lv[5]);
    half4 brdrMask = lerp(Transp, FigMask, FigD > -width);
    bool isBorder = all(brdrMask != Transp);
    if(isBorder) result = borderColor;
    return result;
}

half4 LayerGradientBorder(half lv[VALUES_N], sampler2D lt)
{
    half4 result = Transp;
    half width = lv[1];
    half type = lv[2];
    half2 gradPos = half2(lv[3], lv[4]);
    half2 gradSize = half2(lv[5], lv[6]);
    half angle = lv[7];
    half4 brdrMask = lerp(Transp, FigMask, FigD > -width);
    bool isBorder = all(brdrMask != Transp);

    if(isBorder)
    {
        if(type == 0.0)
        {
            result = tex2D(lt, rotate((Inp.uv - 0.5) / ExpCoeff, angle) + 0.5);
        }
        else if(type == 1.0)
        {
            result = RadialGradient((Inp.uv - 0.5) / ExpCoeff, lt, gradPos - 0.5, gradSize.x, gradSize.y, angle);
        }
        else if(type == 2.0)
        {
            result = DiamondGradient((Inp.uv - 0.5) / ExpCoeff, lt, gradPos - 0.5, gradSize.x, gradSize.y, angle);
        }
    }

    return result;
}

half4 LayerShadow(half lv[VALUES_N], sampler2D lt)
{
    half shInset = lv[1];
    half4 shColor = half4(lv[2], lv[3], lv[4], lv[5]);
    half2 shPos = half2(lv[6], lv[7]);
    half shBlur = lv[8];
    half shSpread = lv[9];
    half2 shHalfSize = HalfSize + shSpread.xx * (shInset == 1.0 ? -1.0 : 1.0);
    half4 shCorners = _CornersRadius + normalize(_CornersRadius) * shSpread * (shInset == 1.0 ? -2.0 : 1.0);

    half shD = sdfRoundBox(Pos - shPos, shHalfSize, shCorners);
    half4 figMaskInv = lerp(White, Transp, FigD < 0);
    
    shD = shD * sign(shInset - 0.5);
    shD += shBlur;
    shD = shD / shBlur;
    half4 shMask = half4(1, 1, 1, clamp(shD, 0.0, 1.0));
    half4 shCropMask = shInset == 1.0 ? FigMask : figMaskInv;
    shMask = lerp(shCropMask, shMask, shCropMask);
    return lerp(Transp, shMask * shColor, shMask);
}

half4 ProcLayer(half layerValues[VALUES_N], sampler2D layerTexture)
{
    const half type = layerValues[0];
    half4 result = Transp;

    if(type == 0.0) result = LayerSolidColor(layerValues, layerTexture);
    if(type == 1.0) result = LayerGradientColor(layerValues, layerTexture);
    if(type == 2.0) result = LayerImage(layerValues, layerTexture);
    if(type == 3.0) result = LayerBorder(layerValues, layerTexture);
    if(type == 4.0) result = LayerGradientBorder(layerValues, layerTexture);
    if(type == 5.0) result = LayerShadow(layerValues, layerTexture);

    return result;
}

// TODO: Weighted corner radiuses clamping.
// TODO: Recalculation of centers with roundings taking into account.
float4 Generate(v2f inp)
{
    FullSize = _Size + _Expansion * 2.0;
    HalfSize = _Size * 0.5;
    ExpCoeff = _Size / FullSize;
    Inp = inp;
    Pos = inp.uv - 0.5;
    Pos *= FullSize;
    FigD = sdfRoundBox(Pos, HalfSize, _CornersRadius);
    FigMask = lerp(Transp, White, FigD < 0);

    half4 result = Transp;

    #ifdef LAYER_0
    half4 layer0 = ProcLayer(_LayerValues0, _LayerTexture0);
    result = lerp(result, layer0, layer0.a);
    #endif

    #ifdef LAYER_1
    half4 layer1 = ProcLayer(_LayerValues1, _LayerTexture1);
    result = lerp(result, layer1, layer1.a);
    #endif

    #ifdef LAYER_2
    half4 layer2 = ProcLayer(_LayerValues2, _LayerTexture2);
    result = lerp(result, layer2, layer2.a);
    #endif

    #ifdef LAYER_3
    half4 layer3 = ProcLayer(_LayerValues3, _LayerTexture3);
    result = lerp(result, layer3, layer3.a);
    #endif

    #ifdef LAYER_4
    half4 layer4 = ProcLayer(_LayerValues4, _LayerTexture4);
    result = lerp(result, layer4, layer4.a);
    #endif

    #ifdef LAYER_5
    half4 layer5 = ProcLayer(_LayerValues5, _LayerTexture5);
    result = lerp(result, layer5, layer5.a);
    #endif

    #ifdef LAYER_6
    half4 layer6 = ProcLayer(_LayerValues6, _LayerTexture6);
    result = lerp(result, layer6, layer6.a);
    #endif

    #ifdef LAYER_7
    half4 layer7 = ProcLayer(_LayerValues7, _LayerTexture7);
    result = lerp(result, layer7, layer7.a);
    #endif

    #ifdef LAYER_8
    half4 layer8 = ProcLayer(_LayerValues8, _LayerTexture8);
    result = lerp(result, layer8, layer8.a);
    #endif

    #ifdef LAYER_9
    half4 layer9 = ProcLayer(_LayerValues9, _LayerTexture9);
    result = lerp(result, layer9, layer9.a);
    #endif

    return result;
}
