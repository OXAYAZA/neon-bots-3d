using NeonBots.UI;
using TMPro;
using UnityEngine;

namespace NeonBots.Screens
{
    public class LoadingScreen : UIScreen
    {
        [SerializeField]
        private TMP_Text loadingText;

        public ProgressBar progressBar;

        public void SwitchText(bool state) => this.loadingText.gameObject.SetActive(state);

        public void SetText(string text) => this.loadingText.text = text;
    }
}
