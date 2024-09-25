using NeonBots.Managers;
using NeonBots.UI.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    public class LogMessage : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private LayoutElement layoutElement;

        [SerializeField]
        private RectTransform inner;

        [SerializeField]
        private Button button;

        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private TMP_Text stackTrace;

        private bool isReady;

        private void OnEnable() => this.button.onClick.AddListener(this.SwitchStackTrace);

        private void OnDisable() => this.button.onClick.RemoveListener(this.SwitchStackTrace);

        protected virtual void Update()
        {
            if(!this.isReady) return;

            var visible = this.IsVisible();

            if(this.inner.gameObject.activeSelf != visible)
            {
                this.layoutElement.minHeight = visible ? 0f : this.rectTransform.rect.height;
                this.inner.gameObject.SetActive(visible);
            }
        }

        public void Init(DebugManager.Log log)
        {
            this.text.text = log.text;
            this.stackTrace.text = log.stackTrace;

            switch(log.type)
            {
                case LogType.Error:
                case LogType.Exception:
                    this.text.color = Color.red;
                    break;
                case LogType.Warning:
                    this.text.color = Color.yellow;
                    break;
            }

            this.isReady = true;
        }

        private void SwitchStackTrace() => this.stackTrace.gameObject.SetActive(!this.stackTrace.gameObject.activeSelf);

        private bool IsVisible() => this.rectTransform.IsVisibleFrom(Camera.main);
    }
}
