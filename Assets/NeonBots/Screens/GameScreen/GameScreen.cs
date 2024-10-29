using NeonBots.Components;
using NeonBots.Managers;
using NeonBots.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class GameScreen : UIScreen
    {
        [SerializeField]
        private Button pauseButton;

        [SerializeField]
        private GameObject touchControl;

        [SerializeField]
        private Bar hpBar;

        private Unit Hero => MainManager.GetManager<GameManager>()?.Hero;

        private LocalConfig localConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            MainManager.SetPause(false);
            this.localConfig = MainManager.GetManager<LocalConfig>();
            this.Refresh();
            this.localConfig.OnLocalValueChanged += this.Refresh;
            this.pauseButton.onClick.AddListener(this.Back);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.localConfig.OnLocalValueChanged -= this.Refresh;
            this.pauseButton.onClick.RemoveListener(this.Back);
        }

        private void Update()
        {
            if(this.Hero == default) return;
            this.hpBar.maxValue = this.Hero.baseHp;
            this.hpBar.Value = this.Hero.hp;
        }

        private void Refresh(string name = null)
        {
            if(name is null or "touch_control")
                this.touchControl.SetActive(this.localConfig.Get<bool>("touch_control"));
        }

        public override void Back()
        {
            MainManager.SetPause(true);
            this.uim.GetScreen<PauseScreen>().Open();
        }
    }
}
