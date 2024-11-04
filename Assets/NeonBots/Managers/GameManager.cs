using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NeonBots.Components;
using NeonBots.Screens;
using UnityEngine;

namespace NeonBots.Managers
{
    public class GameManager : Manager
    {
        [SerializeField]
        private Color heroColor = Color.green;

        [SerializeField]
        private float heroHp = 1000f;

        [SerializeField]
        private List<Unit> heroPrefabs;

        public Unit Hero { get; private set; }

        public Waker wakerPrefab;

        private void Update()
        {
            if(!this.IsReady) return;

            if(this.Hero != default) return;

            this.Dissolve();
            var uiManager = MainManager.GetManager<UIManager>();
            uiManager.GetScreen<GameScreen>().Close();
            uiManager.GetScreen<DeathScreen>().Open();
        }

        public async UniTask Init(SceneData sceneData)
        {
            if(sceneData.locationGenerator != default)
            {
                await sceneData.locationGenerator.Generate();
                sceneData.heroSpawnPosition = sceneData.locationGenerator.GetStartPosition();
            }

            if(this.Hero == default)
            {
                var randomNumber = Random.Range(0, this.heroPrefabs.Count);
                this.Hero = Instantiate(this.heroPrefabs[randomNumber], sceneData.heroSpawnPosition, Quaternion.identity);
                this.Hero.sleeper.enabled = false;
                DontDestroyOnLoad(this.Hero.gameObject);

                Instantiate(this.wakerPrefab, this.Hero.transform);

                this.Hero.fraction = "green";
                this.Hero.color = this.heroColor;
                this.Hero.baseHp = this.heroHp;
                this.Hero.ResetValues();

                Destroy(this.Hero.GetComponent<Controller>());
                var controller = this.Hero.gameObject.AddComponent<InputController>();
                controller.Init();

                var watcher = MainManager.Camera.GetComponent<Watcher>();
                watcher.target = this.Hero.gameObject;
            }
            else
            {
                this.Hero.transform.position = sceneData.heroSpawnPosition;
            }

            this.IsReady = true;
        }

        public void Dissolve()
        {
            if(this.Hero != default) Destroy(this.Hero.gameObject);
            this.IsReady = false;
            this.Hero = default;
        }

        [ContextMenu("Refresh hero")]
        private void RefreshHero()
        {
            if(this.Hero == default) return;
            this.Hero.color = this.heroColor;
            this.Hero.ResetValues();
        }
    }
}
