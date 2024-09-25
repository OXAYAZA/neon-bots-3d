using System;
using UnityEngine;

namespace UIBlock.UIBlock1
{
    [Serializable]
    public class Shadow
    {
        [SerializeField]
        private bool inset;

        public bool Inset
        {
            get => this.inset;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.inset = value;
            }
        }

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
        private Vector2 position;

        public Vector2 Position
        {
            get => this.position;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.position = value;
            }
        }

        [SerializeField, Min(0f)]
        private float blur;

        public float Blur
        {
            get => this.blur;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.blur = value;
            }
        }

        [SerializeField]
        private float spread;

        public float Spread
        {
            get => this.spread;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.spread = value;
            }
        }

        internal Block parent;
    }
}
