Shader "UI/Block"
{
    Properties
    {
        _Size ("Size", Vector) = (1,1,0,0)
        _Expansion ("Expansion", Vector) = (0,0,0,0)

        _BackgroundColor ("BackgroundColor", Color) = (0,0,0,0)

        _BackgroundImage ("BackgroundImage", 2D)  = "white" {}
        _BackgroundImageSizing ("BackgroundImageSizing", Integer) = 0
        _BackgroundImageBlur ("BackgroundImageBlur", Vector) = (0,4.0,32.0,0)

        _BackgroundGradientType ("BackgroundGradientType", Integer) = 0
        _BackgroundGradient ("BackgroundGradient", 2D)  = "white" {}
        _BackgroundGradientPosition ("BackgroundGradientPosition", Vector)  = (0,0,0,0)
        _BackgroundGradientSize ("BackgroundGradientSize", Vector)  = (1,1,0,0)
        _BackgroundGradientAngle ("BackgroundGradientAngle", Float) = 0

        _BorderColor ("BorderColor", Color) = (0,0,0,0)
        _BorderWidth ("BorderWidth", Float) = 0
        _BorderRadius ("BorderRadius", Vector) = (0,0,0,0)
        _BorderGradientType ("BorderGradientType", Integer) = 0
        _BorderGradient ("BorderGradient", 2D)  = "white" {}
        _BorderGradientPosition ("BorderGradientPosition", Vector)  = (0,0,0,0)
        _BorderGradientSize ("BorderGradientSize", Vector)  = (1,1,0,0)
        _BorderGradientAngle ("BorderGradientAngle", Float) = 0

        _ShadowInset ("ShadowInset", Integer) = 0
        _ShadowColor ("ShadowColor", Color) = (0,0,0,0)
        _ShadowPosition ("ShadowPosition", Vector) = (0,0,0,0)
        _ShadowBlur ("ShadowBlur", Float) = 0
        _ShadowSpread ("ShadowSpread", Float) = 0

        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Util.cginc"

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            float4 _ClipRect;

            v2f vert(appdata i)
            {
                v2f o;
                o.worldPosition = i.position;
                o.position = UnityObjectToClipPos(i.position + i.uv1 * float4(_Expansion.x, _Expansion.y, 0.0, 0.0));
                o.uv = i.uv;
                o.color = i.color;
                return o;
            }

            half4 frag(v2f inp) : SV_TARGET
            {
                half4 resultColor = Generate(inp);
                resultColor *= inp.color;

                #ifdef UNITY_UI_CLIP_RECT
                resultColor.a *= UnityGet2DClipping(inp.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(resultColor.a - 0.001);
                #endif

                return resultColor;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Util.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            half3 _BackgroundImageBlur;

            v2f vert(appdata i)
            {
                v2f o;
                o.worldPosition = i.position;
                o.position = UnityObjectToClipPos(i.position);
                o.uv = i.uv;
                o.color = i.color;
                return o;
            }

            half4 frag(v2f inp) : SV_TARGET
            {
                //return SomeBlur(inp.uv, _MainTex, _BackgroundImageBlur.z, _BackgroundImageBlur.y, _BackgroundImageBlur.x);
                return GaussianBlur(_MainTex, inp.uv, _MainTex_TexelSize.xy, _BackgroundImageBlur.x, _BackgroundImageBlur.y);
            }
            ENDCG
        }
    }
}
