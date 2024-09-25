using System;
using UnityEngine;

namespace UIBlock.UIBlock2
{
    [Serializable]
    public class GradientBorder : LayerData
    {
        [Min(0f)]
        public float width;

        public GradientColorType type;
        
        public GradientResolution resolution = GradientResolution.K256;

        public Gradient gradient;

        public Vector2 position = new(0.5f, 0.5f);

        [Min(0f)]
        public Vector2 size = new(0.5f, 0.5f);

        [Range(0f, 360f)]
        public float angle;

        private Texture2D texture;

        public override float[] GetValues(Block2 parent = null)
        {
            var arr = new float[Block2.LayerParamsN];
            arr[0] = 4f;
            arr[1] = this.width;
            arr[2] = (float)this.type;
            arr[3] = this.position.x;
            arr[4] = this.position.y;
            arr[5] = this.size.x;
            arr[6] = this.size.y;
            arr[7] = this.angle / 360f;
            return arr;
        }

        public override Texture2D GetTexture()
        {
            if(this.texture != default) return this.texture;

            this.texture = new(1, (int)this.resolution, TextureFormat.ARGB32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                anisoLevel = 1
            };

            var colors = new Color[(int)this.resolution];
            var div = (float)(int)this.resolution;
            for(var i = 0; i < (int)this.resolution; ++i)
            {
                var t = i / div;
                colors[i] = this.gradient.Evaluate(t);
            }

            this.texture.SetPixels(colors);
            this.texture.Apply(false, false);

            return this.texture;
        }

        public override bool GetEnabling() => this.width != 0f;
    }
}
