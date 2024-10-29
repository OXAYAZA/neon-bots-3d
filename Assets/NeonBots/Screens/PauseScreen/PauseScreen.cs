using Cysharp.Threading.Tasks;
using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class PauseScreen : UIScreen
    {
        [Header("Pause Screen")]
        [SerializeField]
        private Button pauseButton;

        [SerializeField]
        private Button resumeButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private Button optionsButton;

        [SerializeField]
        private GameObject fullscreenButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.pauseButton.onClick.AddListener(this.Back);
            this.resumeButton.onClick.AddListener(this.Back);
            this.exitButton.onClick.AddListener(this.OnExit);
            this.optionsButton.onClick.AddListener(this.uim.GetScreen<OptionsScreen>().Open);
#if !UNITY_EDITOR
            this.fullscreenButton.SetActive(Application.platform is RuntimePlatform.WebGLPlayer);
#endif
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.pauseButton.onClick.RemoveListener(this.Back);
            this.resumeButton.onClick.RemoveListener(this.Back);
            this.exitButton.onClick.RemoveListener(this.OnExit);
            this.optionsButton.onClick.RemoveListener(this.uim.GetScreen<OptionsScreen>().Open);
        }

        private void OnExit() => MainManager.LoadMainMenu().Forget();
    }
}
