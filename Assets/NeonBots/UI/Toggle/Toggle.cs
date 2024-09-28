using UIBlock.UIBlock1;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(Button))]
    public class Toggle : MonoBehaviour
    {
        public Color enabledColor = Color.green;

        public Color disabledColor = Color.red;

        public float handlePosition = 8f;

        private bool state;

        public virtual bool State
        {
            get => this.state;
            set
            {
                this.state = value;
                this.Refresh();
            }
        }

        private Button button;

        private Block handle;

        protected virtual void OnEnable()
        {
            this.button = this.GetComponent<Button>();
            this.handle = this.transform.GetChild(0).GetComponent<Block>();
            this.Refresh();
            this.button.onClick.AddListener(this.Switch);
        }

        protected void OnDisable() => this.button.onClick.RemoveListener(this.Switch);

        [ContextMenu("Switch")]
        protected void Switch() => this.State = !this.State;

        protected void Refresh()
        {
            this.handle.BackgroundColor = this.State ? this.enabledColor : this.disabledColor;
            var handleTransform = (RectTransform)this.handle.transform;
            handleTransform.anchoredPosition = new(this.State ? this.handlePosition : -this.handlePosition,
                handleTransform.anchoredPosition.y);
        }
    }
}
