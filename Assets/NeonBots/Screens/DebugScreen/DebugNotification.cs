using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace NeonBots.Screens
{
    public class DebugNotification : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private float duration = 5f;

        private CancellationTokenSource cts;
        
        public async UniTask Show(string text)
        {
            this.Reset();
            this.text.text = text;
            this.panel.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(this.duration), cancellationToken: this.cts.Token);
            this.panel.SetActive(false);
        }

        public void Reset()
        {
            this.cts?.Cancel();
            this.cts = new();
        }
    }
}
