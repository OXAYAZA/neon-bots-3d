using System.Linq;
using UnityEngine;

namespace NeonBots.UI
{
    public class ResolutionSelect : Select
    {
        protected override void Init()
        {
            this.Value = null;
            this.label.text = "Resolution";
            this.items.Clear();

            foreach(var resolution in Screen.resolutions)
            {
                this.items.Add(new()
                {
                    text = $"{resolution.width}x{resolution.height} {resolution.refreshRateRatio}Hz",
                    value = $"{resolution.width}x{resolution.height}x{resolution.refreshRateRatio}",
                    data = new ()
                    {
                        { "width", resolution.width },
                        { "height", resolution.height },
                        { "refreshRateRatio", resolution.refreshRateRatio }
                    }
                });
            }

            var current = this.items.FirstOrDefault(item =>
                item.value == $"{Screen.currentResolution.width}x{Screen.currentResolution.height}x{Screen.currentResolution.refreshRateRatio}");

            if(current == default) return;

            this.SetActive(current);
        }

        public override void SetActive(Item item)
        {
            base.SetActive(item);

            Screen.SetResolution(
                (int)item.data["width"],
                (int)item.data["height"],
                FullScreenMode.FullScreenWindow,
                (RefreshRate)item.data["refreshRateRatio"]
            );
        }
    }
}
