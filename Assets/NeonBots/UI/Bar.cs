using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    public class Bar : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private ProgressBar progressBar;

        [SerializeField]
        private Image bar;

        [SerializeField]
        private Color fullColor = Color.green;

        [SerializeField]
        private Color emptyColor = Color.red;

        [NonSerialized]
        public float maxValue;

        private float value;

        public float Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.Refresh();
            }
        }

        private void Refresh()
        {
            var progress = this.Value / this.maxValue;
            this.progressBar.Progress = progress;
            this.text.text = $"{this.Value} / {this.maxValue}";
            this.bar.color = Color.Lerp(this.emptyColor, this.fullColor, progress);
        }
    }
}
