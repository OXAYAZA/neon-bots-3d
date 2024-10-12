﻿using Cysharp.Threading.Tasks;
using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class RootScreen : UIScreen
    {
        [Header("Root Screen")]
        [SerializeField]
        private Button playButton;

        [SerializeField]
        private Button optionsButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private GameObject fullscreenButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.playButton.onClick.AddListener(this.OnPlayClick);
            this.optionsButton.onClick.AddListener(this.uim.GetScreen<OptionsScreen>().Open);
            this.exitButton.onClick.AddListener(this.Exit);
#if !UNITY_EDITOR
            this.exitButton.gameObject.SetActive(Application.platform is RuntimePlatform.WindowsPlayer);
            this.fullscreenButton.SetActive(Application.platform is RuntimePlatform.WebGLPlayer);
#endif
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.playButton.onClick.RemoveListener(this.OnPlayClick);
            this.optionsButton.onClick.RemoveListener(this.uim.GetScreen<OptionsScreen>().Open);
            this.exitButton.onClick.RemoveListener(this.Exit);
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

        private void Exit() => Application.Quit();
    }
}
