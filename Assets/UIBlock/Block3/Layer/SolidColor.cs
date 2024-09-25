using UnityEngine;

namespace UIBlock.UIBlock3
{
    public class SolidColor : Layer
    {
        [SerializeField]
        private Color color;

        public Color Color
        {
            get => this.color;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.color = value;
            }
        }

        public override float[] GetValues()
        {
            var arr = new float[Block3.LayerParamsN];
            arr[0] = 0f;
            arr[1] = this.Color.r;
            arr[2] = this.Color.g;
            arr[3] = this.Color.b;
            arr[4] = this.Color.a;
            return arr;
        }

        public override bool GetEnabling() => base.GetEnabling() && this.Color.a != 0f;
    }
}
