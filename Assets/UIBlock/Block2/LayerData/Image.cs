using System;
using UnityEngine;

namespace UIBlock.UIBlock2
{
    // TODO: Image anchor, anchoring modes, rotation angle and pivot.
    [Serializable]
    public class Image : LayerData
    {
        public Texture2D texture;

        public ImageSizingType sizing;

        [Tooltip("The position value is also the anchor value and is measured as a percentage of the block size.")]
        public Vector2 position = new(0.5f, 0.5f);

        [Tooltip("Size parameter is only works in Manual sizing mode.")]
        public Vector2 size;

        [Min(0f), Tooltip("It is better to use small values, 0.03 for example.")]
        public float blurRadius;

        [Min(0f), Tooltip("4 is optimal value for small blur radius.")]
        public float blurQuality = 4f;

        [Min(0), Tooltip("32 is optimal value for small blur radius.")]
        public int blurDirections = 32;

        public override float[] GetValues(Block2 parent)
        {
            var isSet = this.texture != default;
            var texSize = Vector2.zero;

            if(isSet)
            {
                texSize = new(this.texture.width, this.texture.height);

                texSize = this.sizing switch
                {
                    ImageSizingType.Manual => this.size,
                    ImageSizingType.Stretch => parent.size,
                    ImageSizingType.Cover => texSize * Mathf.Max(parent.size.x / texSize.x, parent.size.y / texSize.y),
                    ImageSizingType.Contain => texSize * Mathf.Min(parent.size.x / texSize.x, parent.size.y / texSize.y),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            var arr = new float[Block2.LayerParamsN];
            arr[0] = 2f;
            arr[1] = texSize.x;
            arr[2] = texSize.y;
            arr[3] = this.position.x;
            arr[4] = this.position.y;
            arr[5] = this.blurRadius;
            arr[6] = this.blurQuality;
            arr[7] = this.blurDirections;
            return arr;
        }

        public override Texture2D GetTexture() => this.texture;

        public override bool GetEnabling() => this.texture != default;
    }
}
