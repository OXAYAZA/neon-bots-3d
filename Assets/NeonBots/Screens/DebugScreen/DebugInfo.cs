using NeonBots.Managers;
using TMPro;
using UnityEngine;

namespace NeonBots.Screens
{
    [RequireComponent(typeof(TMP_Text))]
    public class DebugInfo : MonoBehaviour
    {
        private UIManager uiManager;

        private TMP_Text text;

        private void Awake()
        {
            this.text = this.GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            this.uiManager = MainManager.GetManager<UIManager>();
            this.Refresh();
            this.uiManager.OnResize += this.Refresh;
        }

        private void OnDisable()
        {
            this.uiManager.OnResize -= this.Refresh;
        }

        private void Update() => this.Refresh();

        private void Refresh()
        {
            var size = new Vector2(Screen.width, Screen.height);
            var text = $"Screen size: {size.x}x{size.y};\n" +
                $"Base size: {this.uiManager.baseSize};\n" +
                $"Max size: {this.uiManager.maxSize};\n" +
                $"DPI: {Screen.dpi};\n" +
                $"Base DPI: {this.uiManager.baseDpi};\n" +
                $"Scale factor: {this.uiManager.ScaleFactor};\n" +
                $"Scaled size: {this.uiManager.ScaledSize.x}x{this.uiManager.ScaledSize.y};\n";
            this.text.text = text;
        }
    }
}
