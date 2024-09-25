using System;
using UnityEngine;

namespace UIBlock.UIBlock1
{
    [Serializable]
    public class GradientColor
    {
        [SerializeField]
        private GradientType type;

        public GradientType Type
        {
            get => this.type;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.type = value;
            }
        }

        [SerializeField]
        private GradientRes resolution = GradientRes.k256;

        public GradientRes Resolution
        {
            get => this.resolution;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.resolution = value;
            }
        }

        [SerializeField]
        private Gradient gradient;

        public Gradient Gradient
        {
            get => this.gradient;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.gradient = value;
            }
        }

        [SerializeField]
        private Vector2 position = new(0.5f, 0.5f);

        public Vector2 Position
        {
            get => this.position;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.position = value;
            }
        }

        [SerializeField]
        private Vector2 size = new(0.5f, 0.5f);

        public Vector2 Size
        {
            get => this.size;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.size = value;
            }
        }

        [SerializeField, Range(0f, 360f)]
        private float angle;

        public float Angle
        {
            get => this.angle;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.angle = value;
            }
        }

        internal Block parent;

        private Texture2D texture;

        public Texture2D GetTexture()
        {
            if(this.texture != default) return this.texture;

            this.texture = new(1, (int)this.Resolution, TextureFormat.ARGB32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                anisoLevel = 1
            };

            var colors = new Color[(int)this.Resolution];
            var div = (float)(int)this.Resolution;
            for(var i = 0; i < (int)this.Resolution; ++i)
            {
                var t = i / div;
                colors[i] = this.Gradient.Evaluate(t);
            }

            this.texture.SetPixels(colors);
            this.texture.Apply(false, false);

            return this.texture;
        }
    }
}