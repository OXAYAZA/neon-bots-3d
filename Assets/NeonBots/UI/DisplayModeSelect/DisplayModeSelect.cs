using System.Linq;
using UnityEngine;

namespace NeonBots.UI
{
    public class DisplayModeSelect : Select
    {
        protected override void Init()
        {
            this.Value = null;
            this.label.text = "Display Mode";
            this.items.Clear();

            this.items.Add(new()
            {
                text = "Fullscreen",
                value = "fullscreen",
                data = new() { { "mode", FullScreenMode.FullScreenWindow } }
            });

            this.items.Add(new()
            {
                text = "Windowed",
                value = "windowed",
                data = new() { { "mode", FullScreenMode.Windowed } }
            });

            var current = this.items.FirstOrDefault(item =>
                (FullScreenMode)item.data["mode"] == Screen.fullScreenMode);

            if(current == default) return;

            this.SetActive(current);
        }

        public override void SetActive(Item item)
        {
            base.SetActive(item);
            Screen.fullScreenMode = (FullScreenMode)item.data["mode"];
        }
    }
}
