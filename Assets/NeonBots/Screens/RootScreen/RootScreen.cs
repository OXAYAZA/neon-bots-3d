using Cysharp.Threading.Tasks;
using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class RootScreen : UIScreen
    {
        [SerializeField]
        private Button playButton;

        [SerializeField]
        private GameObject fullscreenButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.playButton.onClick.AddListener(this.OnPlayClick);
            this.fullscreenButton.SetActive(
                Application.platform is RuntimePlatform.WebGLPlayer or RuntimePlatform.WindowsPlayer);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.playButton.onClick.RemoveListener(this.OnPlayClick);
        }

        private void OnPlayClick() => this.Play().Forget();

        private async UniTask Play()
        {
            this.uim.SwitchOverlay(true);
            await MainManager.UnloadScene();
            var sceneData = await MainManager.LoadScene("Level-1");
            MainManager.GetManager<GameManager>().Init(sceneData);
            this.uim.GetScreen<GameScreen>().Open();
            this.uim.SwitchOverlay(false);
        }
    }
}
