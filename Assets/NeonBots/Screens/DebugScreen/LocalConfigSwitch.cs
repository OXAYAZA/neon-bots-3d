using NeonBots.Managers;
using NeonBots.Screens;
using UnityEngine;

namespace NeonBots.UI
{
    public class LocalConfigSwitch : SettingSwitch
    {
        [Header("Local Config Switch")]
        [SerializeField]
        private string parameter;

        private LocalConfig localConfig;

        private bool State
        {
            get => this.localConfig.Get<bool>(this.parameter);
            set => this.localConfig.Set(this.parameter, value);
        }

        protected override void OnReady()
        {
            base.OnReady();
            this.localConfig = MainManager.GetManager<LocalConfig>();
            this.Refresh(this.State);
            this.localConfig.OnLocalValueChanged += this.OnValueChanged;
        }

        protected override void Switch() => this.State = !this.State;

        private void OnValueChanged(string name)
        {
            if(name == this.parameter) this.Refresh(this.State);
        }
    }
}
