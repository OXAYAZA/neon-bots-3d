using NeonBots.Managers;
using UIBlock.UIBlock1;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public abstract class SettingSwitch : MonoBehaviour
    {
        [Header("Setting Switch")]
        [SerializeField]
        private Block block;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Color inactiveColor = Color.gray;

        [SerializeField]
        private Color enabledColor = Color.green;

        [SerializeField]
        private Color disabledColor = Color.red;

        private void OnEnable()
        {
            this.block.border.Color = this.inactiveColor;
            this.button.interactable = false;
            if(MainManager.IsReady) this.OnReady();
            else MainManager.OnReady += this.OnReady;
        }

        protected virtual void OnReady()
        {
            MainManager.OnReady -= this.OnReady;
            this.button.interactable = true;
            this.button.onClick.AddListener(this.Switch);
        }

        protected virtual void OnDisable()
        {
            MainManager.OnReady -= this.OnReady;
            this.button.onClick.RemoveListener(this.Switch);
        }

        protected void Refresh(bool value)
        {
            this.block.border.Color = value ? this.enabledColor : this.disabledColor;
        }

        protected abstract void Switch();
    }
}
