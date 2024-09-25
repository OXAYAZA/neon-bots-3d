using UnityEngine;

namespace UIBlock.UIBlock2
{
    public interface ILayerData
    {
        public float[] GetValues(Block2 parent = null);

        public Texture2D GetTexture();

        public Vector2 GetExpansion();

        public bool GetEnabling();
    }
}
