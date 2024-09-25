using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIBlock.UIBlock2.Editor
{
    [CustomPropertyDrawer(typeof(BlockLayer))]
    public class BlockLayerPropDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var rs = root.style;
            var color = new Color(26f / 255f, 26f / 255f, 26f / 255f);
            rs.paddingTop = rs.paddingLeft = rs.paddingRight = rs.paddingBottom = 5f;
            rs.borderTopWidth = rs.borderLeftWidth = rs.borderRightWidth = rs.borderBottomWidth = 1f;
            rs.borderTopColor = rs.borderLeftColor = rs.borderRightColor = rs.borderBottomColor = new(color);
            rs.borderTopLeftRadius = rs.borderTopRightRadius =
                rs.borderBottomLeftRadius = rs.borderBottomRightRadius = 3f;

            var separator = new VisualElement();
            separator.style.marginTop = separator.style.marginBottom = 3f;
            separator.style.borderBottomWidth = 1f;
            separator.style.borderBottomColor = new(color);

            var blockSolidColor = new VisualElement { name = "SolidColor" };
            var blockGradientColor = new VisualElement { name = "GradientColor" };
            var blockImage = new VisualElement { name = "Image" };
            var blockBorder = new VisualElement { name = "Border" };
            var blockGradientBorder = new VisualElement { name = "GradientBorder" };
            var blockShadow = new VisualElement { name = "Shadow" };

            var blocksList = new List<VisualElement>
            {
                blockSolidColor,
                blockGradientColor,
                blockImage,
                blockBorder,
                blockGradientBorder,
                blockShadow
            };

            var layerTypeProp = property.FindPropertyRelative("type");
            var layerTypeField = new PropertyField(layerTypeProp, "Layer Type");
            var layerType = (BlockLayerType)layerTypeProp.intValue;

            root.Add(new PropertyField(property.FindPropertyRelative("name")));
            root.Add(layerTypeField);
            root.Add(separator);

            blockSolidColor.Add(new PropertyField(property.FindPropertyRelative("solidColor.color")));
            root.Add(blockSolidColor);

            blockGradientColor.Add(new PropertyField(property.FindPropertyRelative("gradientColor.type")));
            blockGradientColor.Add(new PropertyField(property.FindPropertyRelative("gradientColor.resolution")));
            blockGradientColor.Add(new PropertyField(property.FindPropertyRelative("gradientColor.gradient")));
            blockGradientColor.Add(new PropertyField(property.FindPropertyRelative("gradientColor.position")));
            blockGradientColor.Add(new PropertyField(property.FindPropertyRelative("gradientColor.size")));
            blockGradientColor.Add(new PropertyField(property.FindPropertyRelative("gradientColor.angle")));
            root.Add(blockGradientColor);

            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.texture")));
            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.sizing")));
            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.size")));
            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.position")));
            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.blurRadius")));
            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.blurQuality")));
            blockImage.Add(new PropertyField(property.FindPropertyRelative("image.blurDirections")));
            root.Add(blockImage);

            blockBorder.Add(new PropertyField(property.FindPropertyRelative("border.width")));
            blockBorder.Add(new PropertyField(property.FindPropertyRelative("border.color")));
            root.Add(blockBorder);

            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.width")));
            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.type")));
            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.resolution")));
            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.gradient")));
            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.position")));
            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.size")));
            blockGradientBorder.Add(new PropertyField(property.FindPropertyRelative("gradientBorder.angle")));
            root.Add(blockGradientBorder);

            blockShadow.Add(new PropertyField(property.FindPropertyRelative("shadow.inset")));
            blockShadow.Add(new PropertyField(property.FindPropertyRelative("shadow.color")));
            blockShadow.Add(new PropertyField(property.FindPropertyRelative("shadow.position")));
            blockShadow.Add(new PropertyField(property.FindPropertyRelative("shadow.blur")));
            blockShadow.Add(new PropertyField(property.FindPropertyRelative("shadow.spread")));
            root.Add(blockShadow);

            layerTypeField.RegisterValueChangeCallback(OnLayerTypeChange);

            SwitchType(layerType);

            return root;

            void OnLayerTypeChange(SerializedPropertyChangeEvent evt)
            {
                SwitchType((BlockLayerType)evt.changedProperty.intValue);
            }

            void SwitchType(BlockLayerType layerType)
            {
                foreach(var block in blocksList)
                    block.style.display = DisplayStyle.None;

                root.Q<VisualElement>(layerType.ToString()).style.display = DisplayStyle.Flex;
            }
        }
    }
}
