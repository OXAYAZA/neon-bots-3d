using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UIBlock.UIBlock1
{
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class Block : MaskableGraphic
    {
        private const string ShaderName = "UI/Block";

        [SerializeField]
        private Color backgroundColor;

        public Color BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                this.changed = true;
                this.backgroundColor = value;
            }
        }

        [SerializeField, Min(0f)]
        private Vector2 expansion;

        public Vector2 Expansion
        {
            get => this.expansion;
            set
            {
                this.changed = true;
                this.expansion = value;
            }
        }

        public BackgroundImage backgroundImage;

        public GradientColor backgroundGradient;

        public Border border;

        public Shadow shadow;

        private Material blockMaterial;

        public override Material material => this.blockMaterial != null
            ? this.blockMaterial
            : this.blockMaterial = new(Shader.Find(ShaderName));

        private Vector2 size;

        private Vector2 lastSize;

        private Vector4 cornersRadius;

        private CanvasScaler parentScaler;

        private float scale = 1f;

        private float lastScale;

        private RenderTexture blurredImage;

        internal bool changed = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.backgroundImage ??= new();
            this.backgroundGradient ??= new();
            this.border ??= new();
            this.shadow ??= new();
            this.backgroundImage.parent = this;
            this.backgroundGradient.parent = this;
            this.border.parent = this;
            this.border.Gradient.parent = this;
            this.shadow.parent = this;
            if(this.IsChanged()) this.Refresh();
        }

        protected virtual void OnGUI()
        {
            if(!Application.isPlaying) return;
            if(this.IsChanged()) this.Refresh();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.Refresh();
        }
#endif

        protected override void OnDidApplyAnimationProperties()
        {
            this.Refresh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.material = null;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if(Application.isPlaying) return;
            if(this.enabled && this.material != null) this.Refresh();
        }

        private void GenerateBlurredTexture()
        {
            var texSize = this.size * this.scale;
            var imgSize = new Vector2(this.backgroundImage.Image.width, this.backgroundImage.Image.height);
            imgSize = this.backgroundImage.Sizing switch
            {
                BgSzType.Stretch => texSize,
                BgSzType.Cover => imgSize * Mathf.Max(texSize.x / imgSize.x, texSize.y / imgSize.y),
                BgSzType.Contain => imgSize * Mathf.Min(texSize.x / imgSize.x, texSize.y / imgSize.y),
                _ => throw new ArgumentOutOfRangeException()
            };

            imgSize *= Mathf.Min(this.backgroundImage.Blur.z / imgSize.x, this.backgroundImage.Blur.z / imgSize.y);

            if(this.blurredImage == default)
            {
                this.blurredImage = new((int)imgSize.x, (int)imgSize.y, 0);
            }
            else
            {
                // TODO: Resizing is not work properly here, but it's required.
                var rta = RenderTexture.active;
                this.blurredImage.Release();
                this.blurredImage.width = (int)imgSize.x;
                this.blurredImage.height = (int)imgSize.y;
                this.blurredImage.Create();
                RenderTexture.active = rta;
            }

            var gaussianKernel = Gaussian.GenerateKernelFlatQuad((byte)this.backgroundImage.Blur.x);
            var s = Gaussian.MaxKernelRadius + 1;
            Array.Resize(ref gaussianKernel, s * s);
            this.material.SetFloatArray(PropID.GaussianKernel, gaussianKernel);
            this.material.SetVector(PropID.BackgroundImageBlur, this.backgroundImage.Blur);

            // TODO: "RenderTexture.Create failed: requested size is too large" exception if texture size is 2160х3840 and larger.
            Graphics.Blit(this.backgroundImage.Image, this.blurredImage, this.material, 1);
        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            this.size = ((RectTransform)this.transform).rect.size;
            this.parentScaler ??= this.GetComponentInParent<CanvasScaler>();
            this.scale = this.parentScaler is null ? 1f : this.parentScaler.scaleFactor;

            var maxRadius = Mathf.Min(this.size.x, this.size.y) * 0.5f;
            this.cornersRadius = new(
                this.border.Radius.x < maxRadius ? this.border.Radius.x : maxRadius,
                this.border.Radius.y < maxRadius ? this.border.Radius.y : maxRadius,
                this.border.Radius.z < maxRadius ? this.border.Radius.z : maxRadius,
                this.border.Radius.w < maxRadius ? this.border.Radius.w : maxRadius
            );

            if(
                this.backgroundImage.Image != default
                && this.size is { x: >= 1f, y: >= 1f }
                && this.backgroundImage.Blur.x != 0f
                && this.backgroundImage.Blur.y != 0f
            )
            {
                this.GenerateBlurredTexture();
            }

            this.SetProps(this.material);
            this.SetProps(this.materialForRendering);
            this.SetVerticesDirty();
            this.SetMaterialDirty();

            this.changed = false;
        }

        private void SetProps(Material material)
        {
            material.SetVector(PropID.Size, this.size);
            material.SetVector(PropID.Expansion, this.Expansion);

            material.SetColor(PropID.BackgroundColor, this.backgroundColor);

            var bgImgSetKw = new LocalKeyword(material.shader, "BACKGROUND_IMAGE_SET");
            var bgGradLinSetKw = new LocalKeyword(material.shader, "BACKGROUND_GRADIENT_LINEAR_SET");
            var bgGradRadSetKw = new LocalKeyword(material.shader, "BACKGROUND_GRADIENT_RADIAL_SET");
            var bgGradDmdSetKw = new LocalKeyword(material.shader, "BACKGROUND_GRADIENT_DIAMOND_SET");
            var shdSetKw = new LocalKeyword(material.shader, "SHADOW_SET");
            var brdrColorSetKw = new LocalKeyword(material.shader, "BORDER_COLOR_SET");
            var brdrGradLinSetKw = new LocalKeyword(material.shader, "BORDER_GRADIENT_LINEAR_SET");
            var brdrGradRadSetKw = new LocalKeyword(material.shader, "BORDER_GRADIENT_RADIAL_SET");
            var brdrGradDmdSetKw = new LocalKeyword(material.shader, "BORDER_GRADIENT_DIAMOND_SET");

            material.SetKeyword(bgImgSetKw, this.backgroundImage.Image != default);
            material.SetKeyword(bgGradLinSetKw, this.backgroundGradient.Type == GradientType.Linear);
            material.SetKeyword(bgGradRadSetKw, this.backgroundGradient.Type == GradientType.Radial);
            material.SetKeyword(bgGradDmdSetKw, this.backgroundGradient.Type == GradientType.Diamond);
            material.SetKeyword(shdSetKw, this.shadow.Color.a != 0f);
            material.SetKeyword(brdrColorSetKw, this.border.Gradient.Type == GradientType.None);
            material.SetKeyword(brdrGradLinSetKw, this.border.Gradient.Type == GradientType.Linear);
            material.SetKeyword(brdrGradRadSetKw, this.border.Gradient.Type == GradientType.Radial);
            material.SetKeyword(brdrGradDmdSetKw, this.border.Gradient.Type == GradientType.Diamond);

            if(this.backgroundImage.Image != default)
            {
                material.SetTexture(PropID.BackgroundImage,
                    this.backgroundImage.Blur.x == 0f || this.backgroundImage.Blur.y == 0f ? this.backgroundImage.Image : this.blurredImage);
                material.SetInteger(PropID.BackgroundImageSizing, (int)this.backgroundImage.Sizing);
            }

            material.SetInteger(PropID.BackgroundGradientType, (int)this.backgroundGradient.Type);

            if(this.backgroundGradient.Type != GradientType.None)
            {
                material.SetTexture(PropID.BackgroundGradient, this.backgroundGradient.GetTexture());
                material.SetVector(PropID.BackgroundGradientPosition, this.backgroundGradient.Position);
                material.SetVector(PropID.BackgroundGradientSize, this.backgroundGradient.Size);
                material.SetFloat(PropID.BackgroundGradientAngle, this.backgroundGradient.Angle / 360f);
            }

            material.SetColor(PropID.BorderColor, this.border.Color);
            material.SetFloat(PropID.BorderWidth, this.border.Width);
            material.SetVector(PropID.BorderRadius, this.cornersRadius);
            material.SetInteger(PropID.BorderGradientType, (int)this.border.Gradient.Type);

            if(this.border.Gradient.Type != GradientType.None)
            {
                material.SetTexture(PropID.BorderGradient, this.border.Gradient.GetTexture());
                material.SetVector(PropID.BorderGradientPosition, this.border.Gradient.Position);
                material.SetVector(PropID.BorderGradientSize, this.border.Gradient.Size);
                material.SetFloat(PropID.BorderGradientAngle, this.border.Gradient.Angle / 360f);
            }

            material.SetInteger(PropID.ShadowInset, this.shadow.Inset ? 1 : 0);
            material.SetColor(PropID.ShadowColor, this.shadow.Color);
            material.SetVector(PropID.ShadowPosition, this.shadow.Position);
            material.SetFloat(PropID.ShadowBlur, this.shadow.Blur);
            material.SetFloat(PropID.ShadowSpread, this.shadow.Spread);
        }

        protected virtual void GenerateHelperUvs(VertexHelper vh)
        {
            var vert = new UIVertex();
            var scaleVec = new[]
            {
                new Vector4(-1f, -1f, 0f, 0f),
                new Vector4(-1f, 1f, 0f, 0f),
                new Vector4(1f, 1f, 0f, 0f),
                new Vector4(1f, -1f, 0f, 0f)
            };

            for(var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.uv1 = scaleVec[i];
                vh.SetUIVertex(vert, i);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            if(this.Expansion != Vector2.zero) this.GenerateHelperUvs(vh);
        }

        protected virtual bool IsChanged()
        {
            this.size = ((RectTransform)this.transform).rect.size;
            this.scale = this.parentScaler is null ? 1f : this.parentScaler.scaleFactor;

            if(this.size != this.lastSize)
            {
                this.changed = true;
                this.lastSize = this.size;
            }

            if(!Mathf.Approximately(this.scale, this.lastScale))
            {
                this.changed = true;
                this.lastScale = this.scale;
            }

            return this.changed;
        }
    }
}
