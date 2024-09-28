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

        private Unit hero;

        private LocalConfig localConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            MainManager.SetGamePauseState(false);
            this.localConfig = MainManager.GetManager<LocalConfig>();
            this.Refresh();
            this.localConfig.OnLocalValueChanged += this.Refresh;
            this.pauseButton.onClick.AddListener(this.Back);
        }

        private void OnDisable()
        {
            this.hero = default;
            this.localConfig.OnLocalValueChanged -= this.Refresh;
            this.pauseButton.onClick.RemoveListener(this.Back);
        }

        private void Update()
        {
            this.hero ??= ((GameObject)MainManager.GetManager<ObjectStorage>().Get("hero"))?.GetComponent<Unit>();
            if(this.hero == default) return;
            this.hpBar.maxValue = this.hero.maxHp;
            this.hpBar.Value = this.hero.hp;
        }

        private void Refresh(string name = null)
        {
            if(name is null or "touch_control")
                this.touchControl.SetActive(this.localConfig.Get<bool>("touch_control"));
        }

        public override void Back()
        {
            MainManager.SetGamePauseState(true);
            this.uim.GetScreen<PauseScreen>().Open();
        }
    }
}
