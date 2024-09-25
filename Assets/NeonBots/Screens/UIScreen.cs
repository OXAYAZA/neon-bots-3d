using System.Collections.Generic;
using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler))]
    public class UIScreen : MonoBehaviour, IScreen
    {
        public bool isPopup;

        public bool isBlockBack;

        public bool staticOrder;

        public Canvas Canvas { get; private set; }

        public CanvasScaler Scaler { get; private set; }

        protected UIManager uim;

        protected virtual void Awake()
        {
            this.uim = MainManager.GetManager<UIManager>();
            this.Canvas = this.GetComponent<Canvas>();
            this.Scaler = this.GetComponent<CanvasScaler>();
            this.Scaler.scaleFactor = this.uim.ScaleFactor;
        }

        protected virtual void OnEnable()
        {
            if(!this.staticOrder) this.Canvas.sortingOrder = this.uim.path.Count - 1;
        }

        public virtual void Switch(bool state = true)
        {
            if(!state) this.OnBeforeDisable();
            this.gameObject.SetActive(state);
        }

        public virtual void Open() => MainManager.GetManager<UIManager>().Open(this);

        public virtual void GoTo() => MainManager.GetManager<UIManager>().GoTo(this);

        public virtual void Back() => MainManager.GetManager<UIManager>().Back();

        public bool IsPopup() => this.isPopup;

        public bool IsBlockBack() => this.isBlockBack;

        public virtual void OnBeforeDisable() { }

        public virtual Dictionary<string, object> GetState() => null;

        public virtual void SetState(Dictionary<string, object> data) { }
    }
}