using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI.Tabs
{
    [DisallowMultipleComponent]
    public class Tab : MonoBehaviour
    {
        public Button button;

        public GameObject content;

        public Tabs parent;

        private void OnEnable() => this.button.onClick.AddListener(this.Activate);

        private void OnDisable() => this.button.onClick.RemoveListener(this.Activate);

        public virtual void Activate()
        {
            this.parent?.DisableAll();
            this.parent?.SetActive(this);
            this.content.SetActive(true);
            this.button.interactable = false;
        }

        public void Deactivate()
        {
            this.content.SetActive(false);
            this.button.interactable = true;
        }
    }
}
