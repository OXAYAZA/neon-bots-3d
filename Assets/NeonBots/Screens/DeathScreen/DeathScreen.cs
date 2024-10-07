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

        private void OnExit() => this.Exit().Forget();

        private async UniTask Exit()
        {
            await MainManager.UnloadScene();
            this.uim.GetScreen<RootScreen>().GoTo();
            var sceneData = await MainManager.LoadScene("Level-0");
            MainManager.Camera.transform.position = sceneData.cameraSpawn.position;
            MainManager.Camera.transform.rotation = sceneData.cameraSpawn.rotation;
        }

        private void OnRestart() => this.Restart().Forget();

        private async UniTask Restart()
        {
            this.uim.SwitchOverlay(true);
            this.Close();
            await MainManager.UnloadScene();
            var sceneData = await MainManager.LoadScene("Level-1");
            MainManager.GetManager<GameManager>().Init(sceneData);
            this.uim.GetScreen<GameScreen>().Open();
            this.uim.SwitchOverlay(false);
        }
    }
}
