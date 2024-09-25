using System;
using UnityEngine;

namespace UIBlock.UIBlock3
{
    public class Image : Layer
    {
        [SerializeField]
        private Texture2D texture;

        public Texture2D Texture
        {
            get => this.texture;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.texture = value;
            }
        }

        [SerializeField]
        private ImageSizingType sizing;

        public ImageSizingType Sizing
        {
            get => this.sizing;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.sizing = value;
            }
        }

        [SerializeField, Tooltip("The position value is also the anchor value and is measured as a percentage of the block size.")]
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

        [SerializeField, Tooltip("Size parameter is only works in Manual sizing mode.")]
        private Vector2 size;

        public Vector2 Size
        {
            get => this.size;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.size = value;
            }
        }

        [SerializeField, Min(0f), Tooltip("It is better to use small values, 0.03 for example.")]
        private float blurRadius;

        public float BlurRadius
        {
            get => this.blurRadius;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.blurRadius = value;
            }
        }

        [SerializeField, Min(0f), Tooltip("4 is optimal value for small blur radius.")]
        private float blurQuality = 4f;

        public float BlurQuality
        {
            get => this.blurQuality;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.blurQuality = value;
            }
        }

        [SerializeField, Min(0), Tooltip("32 is optimal value for small blur radius.")]
        private int blurDirections = 32;

        public int BlurDirections
        {
            get => this.blurDirections;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.blurDirections = value;
            }
        }

        public override float[] GetValues()
        {
            var isSet = this.Texture != default;
            var texSize = Vector2.zero;

            if(isSet)
            {
                texSize = new(this.Texture.width, this.Texture.height);

                texSize = this.Sizing switch
                {
                    ImageSizingType.Manual => this.Size,
                    ImageSizingType.Stretch => this.Parent.size,
                    ImageSizingType.Cover => texSize * Mathf.Max(this.Parent.size.x / texSize.x, this.Parent.size.y / texSize.y),
                    ImageSizingType.Contain => texSize * Mathf.Min(this.Parent.size.x / texSize.x, this.Parent.size.y / texSize.y),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            var arr = new float[Block3.LayerParamsN];
            arr[0] = 2f;
            arr[1] = texSize.x;
            arr[2] = texSize.y;
            arr[3] = this.Position.x;
            arr[4] = this.Position.y;
            arr[5] = this.BlurRadius;
            arr[6] = this.BlurQuality;
            arr[7] = this.BlurDirections;
            return arr;
        }

        public override Texture2D GetTexture() => this.Texture;

        public override bool GetEnabling() => base.GetEnabling() && this.Texture != default;
    }
}
