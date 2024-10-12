using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    [DisallowMultipleComponent]
    public class Select : MonoBehaviour
    {
        [Serializable]
        public class Item
        {
            public string text;

            public string value;

            public Dictionary<string, object> data;
        }

        [Header("Select")]
        [SerializeField]
        private Button button;

        [SerializeField]
        protected TMP_Text label;

        [SerializeField]
        private Transform dropdown;

        [SerializeField]
        private Transform itemsContainer;

        [SerializeField]
        private DropdownItem itemPrefab;

        [SerializeField]
        protected List<Item> items;

        public bool closeOnSelect;

        public string Value { get; protected set; }

        public Item ActiveItem  { get; protected set; }

        public bool IsOpened { get; private set; }

        private List<DropdownItem> instances;

        private void OnEnable()
        {
            this.instances = new();
            this.button.onClick.AddListener(this.Switch);
            this.Init();
        }

        private void OnDisable()
        {
            this.button.onClick.RemoveListener(this.Switch);
        }

        protected virtual void Init()
        {
            if(!string.IsNullOrEmpty(this.Value)) return;
            if(this.items is not null && this.items.Count > 0) this.SetActive(this.items[0]);
        }

        public virtual void SetActive(Item item)
        {
            this.Value = item.value;
            this.label.text = item.text;
            this.ActiveItem = item;
            if(this.closeOnSelect && this.IsOpened) this.Switch();
        }

        private void Switch(bool state)
        {
            if(state == this.IsOpened) return;

            if(state)
            {
                foreach(var item in this.items)
                {
                    var inst = Instantiate(this.itemPrefab, this.itemsContainer);
                    inst.Init(this, item);
                    this.instances.Add(inst);
                }
            }
            else
            {
                foreach(var inst in this.instances) Destroy(inst.gameObject);
                this.instances.Clear();
            }

            this.IsOpened = state;
            this.dropdown.gameObject.SetActive(state);
        }

        private void Switch() => this.Switch(!this.IsOpened);
    }
}
