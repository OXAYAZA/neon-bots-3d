using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    [DisallowMultipleComponent]
    public class DropdownItem : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [field: SerializeField]
        public TMP_Text Label { get; private set; }

        public Select.Item Data { get; private set; }

        private Select parent;

        public void Init(Select parent, Select.Item data)
        {
            this.parent = parent;
            this.Data = data;
            this.Label.text = this.Data.text;
            this.button.onClick.AddListener(this.OnClick);
        }

        private void OnDisable() => this.button.onClick.RemoveListener(this.OnClick);

        private void OnClick() => this.parent.SetActive(this.Data);
    }
}
