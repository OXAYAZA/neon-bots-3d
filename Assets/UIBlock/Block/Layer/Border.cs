using System;
using UnityEngine;

namespace UIBlock.UIBlock1
{
    [Serializable]
    public class Border
    {
        [SerializeField]
        private Color color;

        public Color Color
        {
            get => this.color;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.color = value;
            }
        }

        [SerializeField]
        private GradientColor gradient = new();

        public GradientColor Gradient
        {
            get => this.gradient;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.gradient = value;
            }
        }

        [SerializeField, Min(0f)]
        private float width;

        public float Width
        {
            get => this.width;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.width = value;
            }
        }

        [SerializeField, Min(0f)]
        private Vector4 radius;

        public Vector4 Radius
        {
            get => this.radius;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.radius = value;
            }
        }

        internal Block parent;
    }
}
