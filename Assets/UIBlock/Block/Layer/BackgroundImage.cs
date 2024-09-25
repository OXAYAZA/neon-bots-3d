using System;
using UnityEngine;

namespace UIBlock.UIBlock1
{
    [Serializable]
    public class BackgroundImage
    {
        [SerializeField]
        private Texture2D image;

        public Texture2D Image
        {
            get => this.image;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.image = value;
            }
        }

        [SerializeField]
        private BgSzType sizing;

        public BgSzType Sizing
        {
            get => this.sizing;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.sizing = value;
            }
        }

        [SerializeField, Min(0f), Tooltip("x - radius, y - spread, z - sample size")]
        private Vector3 blur = new(0f, 0f, 70f);

        public Vector3 Blur
        {
            get => this.blur;
            set
            {
                if(this.parent is not null) this.parent.changed = true;
                this.blur = value;
            }
        }

        internal Block parent;
    }
}
