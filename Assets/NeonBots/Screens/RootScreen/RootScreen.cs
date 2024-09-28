using Cysharp.Threading.Tasks;
using NeonBots.Components;
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
        private Unit heroPrefab;

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
            var sceneData = await MainManager.LoadScene("Level-1");
            var storage = MainManager.GetManager<ObjectStorage>();
            var watcher = MainManager.Instance.mainCamera.GetComponent<Watcher>();
            var hero = Instantiate(this.heroPrefab, sceneData.heroSpawn.position, sceneData.heroSpawn.rotation);
            hero.ResetValues();
            var controller = hero.gameObject.AddComponent<InputController>();

            storage.Add("hero", hero.gameObject);
            controller.Init();
            watcher.target = hero.gameObject;
            watcher.enabled = true;

            this.uim.GetScreen<GameScreen>().Open();
            this.uim.SwitchOverlay(false);
        }
    }
}
