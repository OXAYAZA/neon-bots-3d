using System;
using UnityEngine;

namespace UIBlock.UIBlock2
{
    [Serializable]
    public class SolidColor : LayerData
    {
        public Color color;

        public override float[] GetValues(Block2 parent = null)
        {
            var arr = new float[Block2.LayerParamsN];
            arr[0] = 0f;
            arr[1] = this.color.r;
            arr[2] = this.color.g;
            arr[3] = this.color.b;
            arr[4] = this.color.a;
            return arr;
        }

        public override bool GetEnabling() => this.color.a != 0f;
    }
}
