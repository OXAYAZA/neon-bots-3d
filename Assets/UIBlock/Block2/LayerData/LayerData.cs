using UnityEngine;

namespace UIBlock.UIBlock2
{
    public class LayerData : ILayerData
    {
        public virtual float[] GetValues(Block2 parent = null) => new float[Block2.LayerParamsN];

        public virtual Texture2D GetTexture() => null;

        public virtual Vector2 GetExpansion() => Vector2.zero;

        public virtual bool GetEnabling() => true;
    }
}
