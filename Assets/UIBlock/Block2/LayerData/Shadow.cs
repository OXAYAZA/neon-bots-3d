using System;
using UnityEngine;

namespace UIBlock.UIBlock2
{
    [Serializable]
    public class Shadow : LayerData
    {
        public bool inset;

        public Color color;

        public Vector2 position;

        [Min(0f)]
        public float blur;

        public float spread;

        public override float[] GetValues(Block2 parent = null)
        {
            var arr = new float[Block2.LayerParamsN];
            arr[0] = 5f;
            arr[1] = this.inset ? 1f : 0f;
            arr[2] = this.color.r;
            arr[3] = this.color.g;
            arr[4] = this.color.b;
            arr[5] = this.color.a;
            arr[6] = this.position.x;
            arr[7] = this.position.y;
            arr[8] = this.blur;
            arr[9] = this.spread;
            return arr;
        }

        public override Vector2 GetExpansion()
        {
            if(this.inset) return Vector2.zero;

            var baseExpansion = new Vector2(Mathf.Abs(this.position.x), Mathf.Abs(this.position.y));
            var halfBlur = this.blur * 0.5f;
            var blurVector = new Vector2(halfBlur, halfBlur);
            var spreadVector = new Vector2(this.spread, this.spread);
            return baseExpansion + blurVector + spreadVector;
        }

        public override bool GetEnabling() =>
            this.color.a != 0f && (this.position != Vector2.zero || this.blur != 0f || this.spread != 0f);
    }
}
