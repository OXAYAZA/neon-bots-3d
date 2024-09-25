using System;
using UnityEngine;

namespace UIBlock.UIBlock2
{
    [Serializable]
    public class Border : LayerData
    {
        [Min(0f)]
        public float width;

        public Color color;

        public override float[] GetValues(Block2 parent = null)
        {
            var arr = new float[Block2.LayerParamsN];
            arr[0] = 3f;
            arr[1] = this.width;
            arr[2] = this.color.r;
            arr[3] = this.color.g;
            arr[4] = this.color.b;
            arr[5] = this.color.a;
            return arr;
        }

        public override bool GetEnabling() => this.width != 0f && this.color.a != 0f;
    }
}
