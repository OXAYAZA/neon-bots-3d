using System.Collections.Generic;
using NeonBots.Managers;
using NeonBots.Screens;
using UnityEngine;

namespace NeonBots.UI
{
    public class TouchShower : MonoBehaviour
    {
        [SerializeField]
        private RectTransform touchPrefab;

        private DebugScreen debugScreen;

        private LocalConfig localConfig;

        private RectTransform rt;

        private List<RectTransform> touchInstances;

        private bool State => this.localConfig.Get<bool>("show_touches");
        
        private void OnEnable()
        {
            if(MainManager.IsReady) this.OnMainReady();
            else MainManager.OnReady += this.OnMainReady;
        }

        private void OnDisable()
        {
            MainManager.OnReady -= this.OnMainReady;
            this.localConfig.OnLocalValueChanged -= this.Switch;
        }

        private void OnMainReady()
        {
            MainManager.OnReady -= this.OnMainReady;
            this.localConfig ??= MainManager.GetManager<LocalConfig>();
            this.debugScreen ??= MainManager.GetManager<UIManager>().GetScreen<DebugScreen>();
            this.rt ??= this.debugScreen.GetComponent<RectTransform>();
            this.touchInstances ??= new();
            this.Switch();
            this.localConfig.OnLocalValueChanged += this.Switch;
        }

        private void Switch()
        {
            if(this.State) return;
            foreach(var inst in this.touchInstances) inst.gameObject.SetActive(false);
        }

        private void Switch(string name)
        {
            if(name == "show_touches") this.Switch();
        }

        private void Update()
        {
            if(!MainManager.IsReady || !this.State || Input.touchCount <= 0) return;

            for(var i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                if(this.touchInstances.Count < i + 1)
                {
                    var newInstance = Instantiate(this.touchPrefab, this.debugScreen.transform);
                    this.touchInstances.Add(newInstance);
                }

                var touchInstance = this.touchInstances[i];

                if(touch.phase is TouchPhase.Began or TouchPhase.Moved)
                {
                    touchInstance.gameObject.SetActive(true);
                    touchInstance.anchoredPosition =
                        touch.position / this.debugScreen.Scaler.scaleFactor - this.rt.rect.size * this.rt.pivot;
                }

                if(touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    touchInstance.gameObject.SetActive(false);
                }
            }
        }
    }
}
