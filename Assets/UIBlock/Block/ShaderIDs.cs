using UnityEngine;

namespace UIBlock.UIBlock1
{
    public static class PropID
    {
        internal static readonly int Size = Shader.PropertyToID("_Size");
        internal static readonly int Expansion = Shader.PropertyToID("_Expansion");

        internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");

        internal static readonly int BackgroundImage = Shader.PropertyToID("_BackgroundImage");
        internal static readonly int BackgroundImageSizing = Shader.PropertyToID("_BackgroundImageSizing");
        internal static readonly int BackgroundImageBlur = Shader.PropertyToID("_BackgroundImageBlur");
        internal static readonly int GaussianKernel = Shader.PropertyToID("_GaussianKernel");

        internal static readonly int BackgroundGradientType = Shader.PropertyToID("_BackgroundGradientType");
        internal static readonly int BackgroundGradient = Shader.PropertyToID("_BackgroundGradient");
        internal static readonly int BackgroundGradientPosition = Shader.PropertyToID("_BackgroundGradientPosition");
        internal static readonly int BackgroundGradientSize = Shader.PropertyToID("_BackgroundGradientSize");
        internal static readonly int BackgroundGradientAngle = Shader.PropertyToID("_BackgroundGradientAngle");

        internal static readonly int BorderColor = Shader.PropertyToID("_BorderColor");
        internal static readonly int BorderWidth = Shader.PropertyToID("_BorderWidth");
        internal static readonly int BorderRadius = Shader.PropertyToID("_BorderRadius");
        internal static readonly int BorderGradientType = Shader.PropertyToID("_BorderGradientType");
        internal static readonly int BorderGradient = Shader.PropertyToID("_BorderGradient");
        internal static readonly int BorderGradientPosition = Shader.PropertyToID("_BorderGradientPosition");
        internal static readonly int BorderGradientSize = Shader.PropertyToID("_BorderGradientSize");
        internal static readonly int BorderGradientAngle = Shader.PropertyToID("_BorderGradientAngle");

        internal static readonly int ShadowInset = Shader.PropertyToID("_ShadowInset");
        internal static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");
        internal static readonly int ShadowPosition = Shader.PropertyToID("_ShadowPosition");
        internal static readonly int ShadowBlur = Shader.PropertyToID("_ShadowBlur");
        internal static readonly int ShadowSpread = Shader.PropertyToID("_ShadowSpread");
    }
}
