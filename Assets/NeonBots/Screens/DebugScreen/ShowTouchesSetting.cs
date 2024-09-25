using NeonBots.Managers;
using UIBlock.UIBlock1;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    public class ShowTouchesSetting : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Block block;

        private DebugManager debug;

        private void OnEnable()
        {
            this.debug = MainManager.GetManager<DebugManager>();
            this.Refresh(this.debug.showTouches);
            this.debug.OnShowTouchesChange += this.Refresh;
            this.button.onClick.AddListener(this.Switch);
        }

        private void OnDisable()
        {
            this.debug.OnShowTouchesChange -= this.Refresh;
            this.button.onClick.RemoveListener(this.Switch);
        }

        private void Refresh(bool state) => this.block.border.Color = state ? Color.green : Color.white;

        private void Switch() => this.debug.SwitchShowTouches(!this.debug.showTouches);
    }
}
