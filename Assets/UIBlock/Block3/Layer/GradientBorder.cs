using UnityEngine;

namespace UIBlock.UIBlock3
{
    public class GradientBorder : Layer
    {
        [SerializeField, Min(0f)]
        private float width;

        public float Width
        {
            get => this.width;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.width = value;
            }
        }

        [SerializeField]
        private GradientColorType type;

        public GradientColorType Type
        {
            get => this.type;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.type = value;
            }
        }
        
        [SerializeField]
        private GradientResolution resolution = GradientResolution.K256;

        public GradientResolution Resolution
        {
            get => this.resolution;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
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
                if(this.Parent is not null) this.Parent.changed = true;
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
                if(this.Parent is not null) this.Parent.changed = true;
                this.position = value;
            }
        }

        [SerializeField, Min(0f)]
        private Vector2 size = new(0.5f, 0.5f);

        public Vector2 Size
        {
            get => this.size;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
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
                if(this.Parent is not null) this.Parent.changed = true;
                this.angle = value;
            }
        }

        private Texture2D texture;

        public override float[] GetValues()
        {
            var arr = new float[Block3.LayerParamsN];
            arr[0] = 4f;
            arr[1] = this.Width;
            arr[2] = (float)this.Type;
            arr[3] = this.Position.x;
            arr[4] = this.Position.y;
            arr[5] = this.Size.x;
            arr[6] = this.Size.y;
            arr[7] = this.Angle / 360f;
            return arr;
        }

        public override Texture2D GetTexture()
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

        public override bool GetEnabling() => base.GetEnabling() && this.Width != 0f;
    }
}
