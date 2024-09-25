#define PI 3.14159265358979323846
#define TWO_PI 6.28318530718

#pragma multi_compile_local __ BACKGROUND_GRADIENT_LINEAR_SET
#pragma multi_compile_local __ BACKGROUND_GRADIENT_RADIAL_SET
#pragma multi_compile_local __ BACKGROUND_GRADIENT_DIAMOND_SET
#pragma multi_compile_local __ BACKGROUND_IMAGE_SET
#pragma multi_compile_local __ SHADOW_SET
#pragma multi_compile_local __ BORDER_COLOR_SET
#pragma multi_compile_local __ BORDER_GRADIENT_LINEAR_SET
#pragma multi_compile_local __ BORDER_GRADIENT_RADIAL_SET
#pragma multi_compile_local __ BORDER_GRADIENT_DIAMOND_SET

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

half2 _Size;
half2 _Expansion;

half4 _BackgroundColor;

int _BackgroundImageSet;
sampler2D _BackgroundImage;
half4 _BackgroundImage_TexelSize;
int _BackgroundImageSizing;

int _BackgroundGradientType;
sampler2D _BackgroundGradient;
half2 _BackgroundGradientPosition;
half2 _BackgroundGradientSize;
half _BackgroundGradientAngle;

half4 _BorderColor;
half _BorderWidth;
half4 _BorderRadius;
int _BorderGradientType;
sampler2D _BorderGradient;
half2 _BorderGradientPosition;
half2 _BorderGradientSize;
half _BorderGradientAngle;

int _ShadowInset;
half4 _ShadowColor;
half2 _ShadowPosition;
half _ShadowBlur;
half _ShadowSpread;

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

float Rect(half2 pos, half2 center, half2 halfSize)
{
    return length(max(abs(pos - center) - halfSize, 0));
}

// radius.x - top-right
// radius.y - boottom-right
// radius.z - top-left
// radius.w - bottom-left
float sdfRoundBox(float2 p, float2 halfSize, float4 radius)
{
    radius.xy = p.x > 0.0 ? radius.xy : radius.zw;
    radius.x = p.y > 0.0 ? radius.x : radius.y;
    float2 q = abs(p) - halfSize + radius.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - radius.x;
}

half4 RadialGradient(half2 pos, sampler2D tex, half2 center, half r1, half r2, half angle)
{
    half2 uv1 = pos;
    half s = sin(2 * PI * -angle / 360);
    half c = cos(2 * PI * -angle / 360);
    half2x2 rotationMatrix = half2x2(c, -s, s, c);
    rotationMatrix *= 0.5;
    rotationMatrix += 0.5;
    rotationMatrix = rotationMatrix * 2 - 1;
    uv1.xy = mul(uv1.xy - center.xy, rotationMatrix);

    half x =  uv1.x;
    half y =  uv1.y;
    half or1 = r1 / 2;
    half or2 = r2 / 2;
    half2 uv2 = sqrt(x * x / or1 + y * y / or2);
    return tex2D(tex, uv2);
}

half4 DiamondGradient(half2 pos, sampler2D tex, half2 center, half r1, half r2, half angle)
{
    half2 uv1 = pos;
    half s = sin(2 * PI * -angle / 360);
    half c = cos(2 * PI * -angle / 360);
    half2x2 rotationMatrix = half2x2(c, -s, s, c);
    rotationMatrix *= 0.5;
    rotationMatrix += 0.5;
    rotationMatrix = rotationMatrix * 2 - 1;
    uv1.xy = mul(uv1.xy - center.xy, rotationMatrix);

    half x =  uv1.x;
    half y =  uv1.y;
    half or1 = r1 / 2;
    half or2 = r2 / 2;
    half2 uv2 = abs(x) / or1 + abs(y) / or2;
    return tex2D(tex, uv2);
}

half4 SomeBlur(half2 pos, sampler2D tex, half directions, half quality, half2 radius)
{
    // Pixel colour
    half4 Color = tex2D(tex, pos);
    
    // Blur calculations
    for(half d = 0.0; d < TWO_PI; d += TWO_PI / directions)
    {
        for(half i = 1.0 / quality; i <= 1.001; i += 1.0 / quality)
        {
            Color += tex2D(tex, pos + half2(cos(d), sin(d)) * radius * i);
        }
    }

    // Output to screen
    Color /= quality * directions + 1.0;
    return Color;
}

// Gaussian Blur

const static int maxRadius = 18;

// 19x19 elements = kernel with radius 18
half _GaussianKernel[(maxRadius + 1) * (maxRadius + 1)];

half4 GaussianBlur(const sampler2D tex, const half2 uv, const half2 texelSize, int radius, const half spread)
{
    half3 sum = half3(0.0, 0.0, 0.0);

    radius = min(radius, maxRadius);
    const int rp1 = radius + 1;

    for(int y = -radius; y <= radius; ++y)
    {
        for(int x = -radius; x <= radius; ++x)
        {
            half w = _GaussianKernel[abs(y) * rp1 + abs(x)];
            sum += tex2D(tex, uv + fixed2(texelSize.x * x * spread, texelSize.y * y * spread)) * w;
        }
    }

    return half4(sum, 1.0);
}

// TODO: Weighted corner radiuses clamping.
// TODO: Recalculation of centers with roundings taking into account.
half4 Generate(v2f inp)
{
    half2 fullSize = _Size + _Expansion * 2.0;
    half2 halfSize = _Size * 0.5;
    half2 expCoeff = _Size / fullSize;
    half2 pos = inp.uv - 0.5;
    pos *= fullSize;
    half figD = sdfRoundBox(pos, halfSize, _BorderRadius);
    half4 figMask = lerp(Transp, White, figD < 0.0);
    half4 result = lerp(Transp, _BackgroundColor, figMask && _BackgroundColor.a);

    // Background Gradient.
    // TODO: Fix colors mix with opacity.
    // TODO: Multiple gradient layers.
    #ifdef BACKGROUND_GRADIENT_LINEAR_SET
    half4 gradLin = tex2D(_BackgroundGradient, rotate((inp.uv - 0.5) / expCoeff, _BackgroundGradientAngle) + 0.5);
    gradLin = lerp(Transp, gradLin, figMask);
    result = lerp(result, gradLin, gradLin.a);
    #endif

    #ifdef BACKGROUND_GRADIENT_RADIAL_SET
    half4 gradRad = RadialGradient((inp.uv - 0.5) / expCoeff, _BackgroundGradient, _BackgroundGradientPosition - 0.5,
        _BackgroundGradientSize.x, _BackgroundGradientSize.y, 0.0);
    gradRad = lerp(Transp, gradRad, figMask);
    result = lerp(result, gradRad, gradRad.a);
    #endif

    #ifdef BACKGROUND_GRADIENT_DIAMOND_SET
    half4 gradDmd = DiamondGradient((inp.uv - 0.5) / expCoeff, _BackgroundGradient, _BackgroundGradientPosition - 0.5,
        _BackgroundGradientSize.x, _BackgroundGradientSize.y, 0.0);
    gradDmd = lerp(Transp, gradDmd, figMask);
    result = lerp(result, gradDmd, gradDmd.a);
    #endif

    // Background Image.
    // TODO: Fix colors mix with opacity.
    #ifdef BACKGROUND_IMAGE_SET
    half2 imgSize = _BackgroundImage_TexelSize.zw;
    if(_BackgroundImageSizing == 0) imgSize = _Size;
    if(_BackgroundImageSizing == 1) imgSize = imgSize * max(_Size.x / imgSize.x, _Size.y / imgSize.y);
    if(_BackgroundImageSizing == 2) imgSize = imgSize * min(_Size.x / imgSize.x, _Size.y / imgSize.y);
    half2 imgPos = pos + imgSize * 0.5;
    half imgRectD = Rect(pos, 0.0, imgSize * 0.5);
    half4 imgMask = lerp(Transp, White, imgRectD <= 0);
    half4 img = tex2D(_BackgroundImage, imgPos / imgSize);
    img = lerp(Transp, img, imgMask);
    img = lerp(Transp, img, figMask);
    result = lerp(result, img, img.a);
    #endif

    // Shadow.
    // TODO: Fix colors mix with opacity.
    // TODO: Multiple shadows.
    #ifdef SHADOW_SET
    half2 shadowHalfSize = halfSize + _ShadowSpread.xx * (_ShadowInset == 1 ? -1.0 : 1.0);
    half4 shadowRadius = _BorderRadius + normalize(_BorderRadius) * _ShadowSpread * (_ShadowInset == 1 ? -2.0 : 1.0);
    half shD = sdfRoundBox(pos - _ShadowPosition, shadowHalfSize, shadowRadius);
    half4 figMaskInv = lerp(White, Transp, figD < 0);
    
    shD = shD * sign(_ShadowInset - 0.5);
    shD += _ShadowBlur;
    shD += _BorderWidth * _ShadowInset;
    shD = shD / _ShadowBlur;
    half4 shMask = half4(1, 1, 1, clamp(shD, 0.0, 1.0));
    half4 shCropMask = _ShadowInset == 1 ? figMask : figMaskInv;
    shMask = lerp(shCropMask, shMask, shCropMask);
    
    half4 sh = lerp(Transp, shMask * _ShadowColor, shMask);
    result = lerp(result, sh, sh.a);
    #endif

    // Border.
    // TODO: Fix colors mix with opacity.
    // TODO: Separate borders width.
    #ifdef BORDER_COLOR_SET
    half4 brdrMask = lerp(Transp, figMask, figD > -_BorderWidth);
    half4 brdr = lerp(Transp, _BorderColor, brdrMask && _BorderColor.a);
    result = lerp(result, brdr, brdr.a);
    #endif

    #ifdef BORDER_GRADIENT_LINEAR_SET
    half4 brdrGradLinMask = lerp(Transp, figMask, figD > -_BorderWidth);
    half4 brdrGradLin = tex2D(_BorderGradient, rotate((inp.uv - 0.5) / expCoeff, _BorderGradientAngle) + 0.5);
    brdrGradLin = lerp(Transp, brdrGradLin, brdrGradLinMask && brdrGradLin.a);
    result = lerp(result, brdrGradLin, brdrGradLin.a);
    #endif

    #ifdef BORDER_GRADIENT_RADIAL_SET
    half4 brdrGradRadMask = lerp(Transp, figMask, figD > -_BorderWidth);
    half4 brdrGradRad = RadialGradient((inp.uv - 0.5) / expCoeff, _BorderGradient, _BorderGradientPosition - 0.5,
        _BorderGradientSize.x, _BorderGradientSize.y, _BorderGradientAngle);
    brdrGradRad = lerp(Transp, brdrGradRad, brdrGradRadMask && brdrGradRad.a);
    result = lerp(result, brdrGradRad, brdrGradRad.a);
    #endif

    #ifdef BORDER_GRADIENT_DIAMOND_SET
    half4 brdrGradDmdMask = lerp(Transp, figMask, figD > -_BorderWidth);
    half4 brdrGradDmd = DiamondGradient((inp.uv - 0.5) / expCoeff, _BorderGradient, _BorderGradientPosition - 0.5,
        _BorderGradientSize.x, _BorderGradientSize.y, _BorderGradientAngle);
    brdrGradDmd = lerp(Transp, brdrGradDmd, brdrGradDmdMask && brdrGradDmd.a);
    result = lerp(result, brdrGradDmd, brdrGradDmd.a);
    #endif

    return result;
}
