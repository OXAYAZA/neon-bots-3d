using UnityEngine;

namespace UIBlock.UIBlock3
{
    [ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public abstract class Layer : MonoBehaviour
    {
        private Block3 parent;

        protected Block3 Parent
        {
            get => this.parent ??= this.GetComponent<Block3>();
            private set => this.parent = value;
        }

        protected virtual void OnValidate()
        {
            if(Application.isPlaying) return;
            this.Parent ??= this.GetComponent<Block3>();
            this.Parent.Refresh();
        }

        protected void OnDidApplyAnimationProperties()
        {
            this.Parent.Refresh();
        }

        public virtual float[] GetValues() => new float[Block3.LayerParamsN];

        public virtual Texture2D GetTexture() => null;

        public virtual Vector2 GetExpansion() => Vector2.zero;

        public virtual bool GetEnabling() => this.gameObject.activeSelf && this.enabled;
    }
}
