using Cysharp.Threading.Tasks;
using NeonBots.Managers;
using NeonBots.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class DebugScreen : UIScreen
    {
        [Header("Debug Screen")]
        [SerializeField]
        private GameObject window;

        [SerializeField]
        private Logs logs;

        [SerializeField]
        private DebugNotification notification;

        [SerializeField]
        private Button switchButton;

        [SerializeField]
        private TMP_Text statusText;

        private DebugManager debug;

        protected override void Awake()
        {
            this.debug = MainManager.GetManager<DebugManager>();
            this.UpdateStatus(this.debug.Status);

            foreach(var log in this.debug.logs) this.AddLog(log);

            this.debug.OnLogAdd += this.AddLog;
            this.debug.OnStatusChange += this.UpdateStatus;
            this.switchButton.onClick.AddListener(this.SwitchWindow);

            base.Awake();
        }

        private void OnDestroy()
        {
            this.debug.OnLogAdd -= this.AddLog;
            this.debug.OnStatusChange -= this.UpdateStatus;
            this.switchButton.onClick.RemoveListener(this.SwitchWindow);
        }

        public void OpenWindow()
        {
            if(!this.window.activeSelf) this.window.SetActive(true);
        }

        public void CloseWindow()
        {
            if(this.window.activeSelf) this.window.SetActive(false);
        }

        public void SwitchWindow() => this.window.SetActive(!this.window.activeSelf);

        private void UpdateStatus(LogType status) =>
            this.statusText.text = status switch
            {
                LogType.Warning => "\u26a0\ufe0f",
                LogType.Error => "\u26d4\ufe0f",
                _ => "\u2705"
            };

        private void AddLog(DebugManager.Log log)
        {
            this.logs.Add(log);
            if(log.type is LogType.Error or LogType.Exception) this.notification.Show(log.text).Forget();
        }
    }
}
