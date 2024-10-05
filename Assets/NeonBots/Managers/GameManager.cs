using System.Collections.Generic;
using NeonBots.Components;
using NeonBots.Screens;
using UnityEngine;

namespace NeonBots.Managers
{
    public class GameManager : Manager
    {
        [field: SerializeField]
        public Unit Hero { get; private set; }

        [SerializeField]
        private List<Unit> heroPrefabs;

        private void Update()
        {
            if(!this.IsReady) return;

            if(this.Hero != default) return;

            this.Dissolve();
            var uiManager = MainManager.GetManager<UIManager>();
            uiManager.GetScreen<GameScreen>().Close();
            uiManager.GetScreen<DeathScreen>().Open();
        }

        public Unit Init(SceneData sceneData)
        {
            var randomNumber = Random.Range(0, this.heroPrefabs.Count);
            this.Hero = Instantiate(this.heroPrefabs[randomNumber],
                sceneData.heroSpawn.position, sceneData.heroSpawn.rotation);
            DontDestroyOnLoad(this.Hero.gameObject);

            this.Hero.fraction = "green";
            this.Hero.color = Color.green;
            // this.Hero.baseHp = 1000f;
            // this.Hero.ResetValues();

            Destroy(this.Hero.GetComponent<Controller>());
            var controller = this.Hero.gameObject.AddComponent<InputController>();
            controller.Init();

            var watcher = MainManager.Camera.GetComponent<Watcher>();
            watcher.target = this.Hero.gameObject; 
            
            this.IsReady = true;
            return this.Hero;
        }

        public void Dissolve()
        {
            if(this.Hero != default) Destroy(this.Hero.gameObject);
            this.IsReady = false;
            this.Hero = default;
        }
    }
}
