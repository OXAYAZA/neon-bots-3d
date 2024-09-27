using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.Screens
{
    public class GameScreen : UIScreen
    {
        [SerializeField]
        private GameObject touchControl;

        private LocalConfig localConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.localConfig = MainManager.GetManager<LocalConfig>();
            this.Refresh();
            this.localConfig.OnLocalValueChanged += this.Refresh;
        }

        private void OnDisable()
        {
            this.localConfig.OnLocalValueChanged -= this.Refresh;
        }

        private void Refresh(string name = null)
        {
            if(name is null or "touch_control")
                this.touchControl.SetActive(this.localConfig.Get<bool>("touch_control"));
        }
    }
}
