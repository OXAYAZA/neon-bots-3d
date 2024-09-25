using UnityEngine;

namespace UIBlock.UIBlock3
{
    public class Border : Layer
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
            arr[0] = 3f;
            arr[1] = this.Width;
            arr[2] = this.color.r;
            arr[3] = this.color.g;
            arr[4] = this.color.b;
            arr[5] = this.color.a;
            return arr;
        }

        public override bool GetEnabling() => base.GetEnabling() && this.Width != 0f && this.color.a != 0f;
    }
}
