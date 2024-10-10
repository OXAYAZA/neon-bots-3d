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

        private void Refresh()
        {
            var text = $"Screen size: {Screen.width}x{Screen.height}\n" +
            $"DPI: {Screen.dpi}\n" +
            $"Base size: {this.uiManager.baseSize}\n" +
            $"Scale: {this.uiManager.ScaleFactor}";
            this.text.text = text;
        }
    }
}
