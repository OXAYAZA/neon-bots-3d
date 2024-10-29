using Cysharp.Threading.Tasks;
using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class DeathScreen : UIScreen
    {
        [Header("Death Screen")]
        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button exitButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.restartButton.onClick.AddListener(this.OnRestart);
            this.exitButton.onClick.AddListener(this.OnExit);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.restartButton.onClick.RemoveListener(this.OnRestart);
            this.exitButton.onClick.RemoveListener(this.OnExit);
        }

        private void OnExit() => MainManager.LoadMainMenu().Forget();

        private void OnRestart()
        {
            this.Close();
            MainManager.LoadLevel().Forget();
        }
    }
}
