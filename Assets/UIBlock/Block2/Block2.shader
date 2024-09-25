Shader "UI/Block2"
{
    Properties
    {
        _CornersRadius ("CornersRadius", Vector) = (0,0,0,0)
        _Size ("Size", Vector) = (0,0,0,0)
        _Expansion ("Expansion", Vector) = (0,0,0,0)

        _LayerTexture0 ("LayerTexture0", 2D)  = "white" {}
        _LayerTexture1 ("LayerTexture1", 2D)  = "white" {}
        _LayerTexture2 ("LayerTexture2", 2D)  = "white" {}
        _LayerTexture3 ("LayerTexture3", 2D)  = "white" {}
        _LayerTexture4 ("LayerTexture4", 2D)  = "white" {}
        _LayerTexture5 ("LayerTexture5", 2D)  = "white" {}
        _LayerTexture6 ("LayerTexture6", 2D)  = "white" {}
        _LayerTexture7 ("LayerTexture7", 2D)  = "white" {}
        _LayerTexture8 ("LayerTexture8", 2D)  = "white" {}
        _LayerTexture9 ("LayerTexture9", 2D)  = "white" {}

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
    }
}
