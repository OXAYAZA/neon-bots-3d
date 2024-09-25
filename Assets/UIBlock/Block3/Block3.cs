using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UIBlock.UIBlock3
{
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class Block3 : MaskableGraphic
    {
        private const string ShaderName = "UI/Block2";

        // This value is duplicated in Util.cginc file as VALUES_N
        internal const int LayerParamsN = 10;

        private static readonly int CornersRadiusId = Shader.PropertyToID("_CornersRadius");

        private static readonly int SizeId = Shader.PropertyToID("_Size");

        private static readonly int ExpansionId = Shader.PropertyToID("_Expansion");

        [SerializeField, Min(0f), Tooltip("X - top-right; Y - bottom-right; Z - top-left; W - bottom-left")]
        private Vector4 cornersRadius;

        public Vector4 CornersRadius
        {
            get => this.cornersRadius;
            set
            {
                this.changed = true;
                this.cornersRadius = value;
            }
        }

        private Layer[] layers;

        private Material blockMaterial;

        public override Material material => this.blockMaterial != null
            ? this.blockMaterial
            : this.blockMaterial = new(Shader.Find(ShaderName));

        internal Vector2 size;

        private Vector2 expansion;

        internal bool changed = true;

        protected override void OnEnable()
        {
            base.OnEnable();
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
            if(this.enabled && this.material != null) this.Refresh();
        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            this.size = ((RectTransform)this.transform).rect.size;
            this.expansion = Vector2.zero;

            this.layers = this.GetComponents<Layer>();

            foreach(var layer in this.layers)
            {
                var expansion = layer.GetExpansion();
                this.expansion.x = expansion.x > this.expansion.x ? expansion.x : this.expansion.x;
                this.expansion.y = expansion.y > this.expansion.y ? expansion.y : this.expansion.y;
            }

            this.SetMaterialProps(this.material);
            this.SetMaterialProps(this.materialForRendering);
            this.SetVerticesDirty();
            this.SetMaterialDirty();
        }

        private void SetMaterialProps(Material material)
        {
            var maxRadius = Mathf.Min(this.size.x, this.size.y) * 0.5f;
            var cornersRadius = new Vector4(
                this.CornersRadius.x < maxRadius ? this.CornersRadius.x : maxRadius,
                this.CornersRadius.y < maxRadius ? this.CornersRadius.y : maxRadius,
                this.CornersRadius.z < maxRadius ? this.CornersRadius.z : maxRadius,
                this.CornersRadius.w < maxRadius ? this.CornersRadius.w : maxRadius
            );

            material.SetVector(CornersRadiusId, cornersRadius);
            material.SetVector(SizeId, this.size);
            material.SetVector(ExpansionId, this.expansion);

            if(this.layers is null) return;

            // Pay attention, only 10 layers are provided
            for(var i = 0; i < 10; i++)
            {
                var isExist = i < this.layers.Length;
                var values = isExist ? this.layers[i].GetValues() : new float[11];
                var tex = isExist ? this.layers[i].GetTexture() : null;
            
                var layerKw = new LocalKeyword(material.shader, $"LAYER_{i}");
                material.SetKeyword(layerKw, isExist && this.layers[i].GetEnabling());
            
                material.SetFloatArray(Shader.PropertyToID($"_LayerValues{i}"), values);
                material.SetTexture(Shader.PropertyToID($"_LayerTexture{i}"), tex);
            }
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
            if(this.expansion != Vector2.zero) this.GenerateHelperUvs(vh);
        }

        protected virtual bool IsChanged() => this.changed;
    }
}
