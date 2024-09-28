using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeonBots.Screens;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeonBots.Managers
{
    public class MainManager : MonoBehaviour
    {
        public static Scene MainScene { get; private set; }

        public Camera mainCamera;

        public static MainManager Instance { get; private set; }

        public static bool IsReady { get; private set; }

        public static bool AppQuitting { get; private set; }

        public static bool AppPaused { get; private set; }

        public static bool GamePaused { get; private set; }

        public static event Action OnReady;

        public static CancellationTokenSource MainCts { get; private set; }

        public Dictionary<Type, Manager> managers;

        private void Awake()
        {
            if(Instance is not null)
                throw new InvalidOperationException("[MainManager] Should be only one instance of MainManager.");

            Instance = this;
            MainScene = SceneManager.GetActiveScene();
        }

        private void Start()
        {
            // Set target framerate to reduce app lags
            Application.targetFrameRate = 60;

            // Disable multitouch
            Input.multiTouchEnabled = false;

            this.Init().Forget();
        }

        private void Update()
        {
            if(!IsReady) return;
            if(Input.GetKeyDown(KeyCode.Escape)) GetManager<UIManager>().path.Last().screen.Back();
        }

        private void OnDestroy()
        {
            AppQuitting = true;
            MainCts?.Cancel();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("[MainManager] Quit");
            AppQuitting = true;
            MainCts?.Cancel();
        }

        private void OnApplicationPause(bool state)
        {
            Debug.Log($"[MainManager] Pause: {state}");
            AppPaused = state;
        }

        private async UniTask Init()
        {
            MainCts = new();

            this.managers = new();
            foreach(var manager in this.GetComponents<Manager>()) this.managers.Add(manager.GetType(), manager);

            var localConfig = GetManager<LocalConfig>();
            var uiManager = GetManager<UIManager>();
            var loadingScreen = uiManager.GetScreen<LoadingScreen>();
            var progressBar = loadingScreen.progressBar;

            loadingScreen.Switch();
            loadingScreen.SetText("Loading...");
            progressBar.Animate(0.8f, 30f, MainCts.Token).Forget();
            localConfig.Init();
            uiManager.GetScreen<DebugScreen>().Switch(localConfig.Get<bool>("console"));

            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                localConfig.Set("touch_control", true);
            }

            var sceneData = await LoadScene("Level-0");
            Instance.mainCamera.transform.position = sceneData.cameraSpawn.position;
            Instance.mainCamera.transform.rotation = sceneData.cameraSpawn.rotation;

            await UniTask.Delay(10, cancellationToken: MainCts.Token);

            IsReady = true;
            OnReady?.Invoke();
            await progressBar.Animate(1f, 0.9f, MainCts.Token);
            await UniTask.Delay(1000, cancellationToken: MainCts.Token);
            uiManager.GetScreen<RootScreen>().Open();
            loadingScreen.Switch(false);
        }

        public static T GetManager<T>() where T : Manager
        {
            if(Instance is null) return null;

            if(!Instance.managers.TryGetValue(typeof(T), out var manager))
                throw new InvalidOperationException($"No child manager of type {typeof(T)}.");

            return (T)manager;
        }

        public static async UniTask<SceneData> LoadScene(string name)
        {
            await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
            return GameObject.Find("SceneData").GetComponent<SceneData>();
        }

        public static async UniTask UnloadScene()
        {
            await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.SetActiveScene(MainScene);
        }

        public static void SetGamePauseState(bool isPaused)
        {
            if(GamePaused == isPaused) return;

            Time.timeScale = isPaused ? 0 : 1;
            GamePaused = isPaused;
            Debug.Log($"[MainManager] Game {(isPaused ? "Paused" : "Resumed")}");
        }
    }
}
