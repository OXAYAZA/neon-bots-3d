using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UIBlock.UIBlock2
{
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class Block2 : MaskableGraphic
    {
        private const string ShaderName = "UI/Block2";

        // This value is duplicated in Util.cginc file as VALUES_N
        internal const int LayerParamsN = 10;

        private static readonly int CornersRadius = Shader.PropertyToID("_CornersRadius");

        private static readonly int Size = Shader.PropertyToID("_Size");

        private static readonly int Expansion = Shader.PropertyToID("_Expansion");

        [Min(0f), Tooltip("X - top-right; Y - bottom-right; Z - top-left; W - bottom-left")]
        public Vector4 cornersRadius;

        public List<BlockLayer> layers;

        private Material blockMaterial;

        public override Material material => this.blockMaterial != null
            ? this.blockMaterial
            : this.blockMaterial = new(Shader.Find(ShaderName));

        internal Vector2 size;

        private Vector2 expansion;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.Refresh();
        }

        protected virtual void OnGUI()
        {
            if(!Application.isPlaying) return;
            // if(!this.IsChanged()) return;
            this.Refresh();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if(Application.isPlaying) return;
            this.Refresh();
        }
#endif

        protected override void OnDidApplyAnimationProperties()
        {
            // if(!this.IsChanged()) return;
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

            foreach(var layerExpansion in this.layers.Select(layer => layer.GetExpansion()))
            {
                this.expansion.x = layerExpansion.x > this.expansion.x ? layerExpansion.x : this.expansion.x;
                this.expansion.y = layerExpansion.y > this.expansion.y ? layerExpansion.y : this.expansion.y;
            }

            this.SetMaterialProps(this.material);
            this.SetMaterialProps(this.materialForRendering);
            this.SetVerticesDirty();
            this.SetMaterialDirty();
        }

        private void SetMaterialProps(Material material)
        {
            material.SetVector(CornersRadius, this.cornersRadius);
            material.SetVector(Size, this.size);
            material.SetVector(Expansion, this.expansion);

            if(this.layers is null) return;

            // Pay attention, only 10 layers are provided
            for(var i = 0; i < 10; i++)
            {
                var isExist = i < this.layers.Count;
                var values = isExist ? this.layers[i].GetValues(this) : new float[11];
                var tex = isExist ? this.layers[i].GetTexture() : null;

                var layerKw = new LocalKeyword(material.shader, $"LAYER_{i}");
                material.SetKeyword(layerKw, isExist && this.layers[i].GetEnabling());

                material.SetFloatArray(Shader.PropertyToID($"_LayerValues{i}"), values);
                material.SetTexture(Shader.PropertyToID($"_LayerTexture{i}"), tex);
            }
        }

        public BlockLayer GetLayer(string name) => this.layers?.Find(layer => layer.name == name);

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
    }
}
