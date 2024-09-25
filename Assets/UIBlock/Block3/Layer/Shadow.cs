using UnityEngine;

namespace UIBlock.UIBlock3
{
    public class Shadow : Layer
    {
        [SerializeField]
        public bool inset;

        public bool Inset
        {
            get => this.inset;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.inset = value;
            }
        }

        [SerializeField]
        public Color color;

        public Color Color
        {
            get => this.color;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.color = value;
            }
        }

        [SerializeField]
        public Vector2 position;

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
        public float blur;

        public float Blur
        {
            get => this.blur;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.blur = value;
            }
        }

        [SerializeField]
        public float spread;

        public float Spread
        {
            get => this.spread;
            set
            {
                if(this.Parent is not null) this.Parent.changed = true;
                this.spread = value;
            }
        }

        public override float[] GetValues()
        {
            var arr = new float[Block3.LayerParamsN];
            arr[0] = 5f;
            arr[1] = this.Inset ? 1f : 0f;
            arr[2] = this.Color.r;
            arr[3] = this.Color.g;
            arr[4] = this.Color.b;
            arr[5] = this.Color.a;
            arr[6] = this.Position.x;
            arr[7] = this.Position.y;
            arr[8] = this.Blur;
            arr[9] = this.Spread;
            return arr;
        }

        public override Vector2 GetExpansion()
        {
            if(this.Inset) return Vector2.zero;

            var baseExpansion = new Vector2(Mathf.Abs(this.Position.x), Mathf.Abs(this.Position.y));
            var halfBlur = this.Blur * 0.5f;
            var blurVector = new Vector2(halfBlur, halfBlur);
            var spreadVector = new Vector2(this.Spread, this.Spread);
            return baseExpansion + blurVector + spreadVector;
        }

        public override bool GetEnabling() =>
            base.GetEnabling() && this.Color.a != 0f
            && (this.Position != Vector2.zero || this.Blur != 0f || this.Spread != 0f);
    }
}
