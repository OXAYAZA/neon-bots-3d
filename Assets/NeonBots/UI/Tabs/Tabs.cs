using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.UI.Tabs
{
    public class Tabs : MonoBehaviour
    {
        [SerializeField]
        private List<Tab> tabs;

        public int activeTab;

        private void OnEnable() => this.tabs[this.activeTab].Activate();

        public void DisableAll()
        {
            foreach(var tab in this.tabs) tab.Deactivate();
        }

        public void SetActive(Tab tab) => this.activeTab = this.tabs.IndexOf(tab);

        public void Activate(int index) => this.tabs[index].Activate();
    }
}
