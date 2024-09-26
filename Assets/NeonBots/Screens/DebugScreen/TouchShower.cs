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

        private DebugManager debugManager;

        private RectTransform rt;

        private List<RectTransform> touchInstances;
        
        private void OnEnable()
        {
            if(MainManager.IsReady) this.OnMainReady();
            else MainManager.OnReady += this.OnMainReady;
        }

        private void OnDisable()
        {
            MainManager.OnReady -= this.OnMainReady;
            if(this.debugManager is not null) this.debugManager.OnShowTouchesChange += this.Switch;
        }

        private void OnMainReady()
        {
            MainManager.OnReady -= this.OnMainReady;
            this.debugManager ??= MainManager.GetManager<DebugManager>();
            this.debugScreen ??= MainManager.GetManager<UIManager>().GetScreen<DebugScreen>();
            this.rt ??= this.debugScreen.GetComponent<RectTransform>();
            this.touchInstances ??= new();
            this.Switch(this.debugManager.showTouches);
            this.debugManager.OnShowTouchesChange += this.Switch;
        }

        private void Switch(bool state)
        {
            if(!state)
                foreach(var inst in this.touchInstances)
                    inst.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(!MainManager.IsReady || !this.debugManager.showTouches || Input.touchCount <= 0) return;

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
