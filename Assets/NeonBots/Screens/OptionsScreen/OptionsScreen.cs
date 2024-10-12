using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.Screens
{
    public class OptionsScreen : UIScreen
    {
        [Header("Options Screen")]
        [SerializeField]
        private Button backButton;

        [SerializeField]
        private GameObject displayModeOption;

        [SerializeField]
        private GameObject resolutionOption;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.Refresh();
            this.uim.OnResize += this.Refresh;
            this.backButton.onClick.AddListener(this.Back);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.uim.OnResize -= this.Refresh;
            this.backButton.onClick.RemoveListener(this.Back);
        }

        private void Refresh()
        {
#if !UNITY_EDITOR
            this.displayModeOption.SetActive(
                Application.platform is RuntimePlatform.WindowsPlayer);
            this.resolutionOption.SetActive(
                Application.platform is RuntimePlatform.WindowsPlayer && Screen.fullScreen);
#endif
        }
    }
}
