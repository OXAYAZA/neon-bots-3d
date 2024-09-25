using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIBlock.UIBlock2
{
    // TODO: Bug with serialization, when creating new layers all fields are empty, default values are not set.
    [Serializable]
    public class BlockLayer
    {
        public string name;

        public BlockLayerType type;

        public SolidColor solidColor = new();

        public GradientColor gradientColor = new();

        public Image image = new();

        public Border border = new();

        public GradientBorder gradientBorder = new();

        public Shadow shadow = new();

        private Dictionary<BlockLayerType, ILayerData> types;

        public BlockLayer()
        {
            this.types = new()
            {
                { BlockLayerType.SolidColor, this.solidColor },
                { BlockLayerType.GradientColor, this.gradientColor },
                { BlockLayerType.Image, this.image },
                { BlockLayerType.Border, this.border },
                { BlockLayerType.GradientBorder, this.gradientBorder },
                { BlockLayerType.Shadow, this.shadow }
            };
        }

        public float[] GetValues(Block2 parent) => this.types[this.type].GetValues(parent);

        public Texture2D GetTexture() => this.types[this.type].GetTexture();

        public Vector2 GetExpansion() => this.types[this.type].GetExpansion();

        public bool GetEnabling() => this.types[this.type].GetEnabling();
    }
}
