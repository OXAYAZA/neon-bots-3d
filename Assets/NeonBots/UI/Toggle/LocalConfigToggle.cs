using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.UI
{
    public class LocalConfigToggle : Toggle
    {
        [SerializeField]
        private string parameter;

        private LocalConfig localConfig;

        public override bool State
        {
            get => this.localConfig.Get<bool>(this.parameter);
            set => this.localConfig.Set(this.parameter, value);
        }

        protected override void OnEnable()
        {
            this.localConfig = MainManager.GetManager<LocalConfig>();
            base.OnEnable();
            this.localConfig.OnLocalValueChanged += this.OnValueChanged;
        }

        private void OnValueChanged(string name)
        {
            if(name == this.parameter) this.Refresh();
        }
    }
}
