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

        protected override void OnEnable()
        {
            base.OnEnable();
            this.playButton.onClick.AddListener(this.OnPlayClick);
        }

        private void OnDisable()
        {
            this.playButton.onClick.RemoveListener(this.OnPlayClick);
        }

        private void OnPlayClick() => this.Play().Forget();

        private async UniTask Play()
        {
            this.uim.SwitchOverlay(true);
            await MainManager.UnloadScene();
            await MainManager.LoadScene("Level-1");
            this.uim.GetScreen<GameScreen>().Open();
            this.uim.SwitchOverlay(false);
        }
    }
}
