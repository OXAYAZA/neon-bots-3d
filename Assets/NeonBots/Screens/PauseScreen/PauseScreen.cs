using Cysharp.Threading.Tasks;
using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class PauseScreen : UIScreen
    {
        [SerializeField]
        private Button pauseButton;

        [SerializeField]
        private Button resumeButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private GameObject fullscreenButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.pauseButton.onClick.AddListener(this.Back);
            this.resumeButton.onClick.AddListener(this.Back);
            this.exitButton.onClick.AddListener(this.OnExit);
            this.fullscreenButton.SetActive(Application.platform == RuntimePlatform.WebGLPlayer);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.pauseButton.onClick.RemoveListener(this.Back);
            this.resumeButton.onClick.RemoveListener(this.Back);
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
            MainManager.GetManager<GameManager>().Dissolve();
        }
    }
}
