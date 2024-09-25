using NeonBots.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    public class Logs : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private Button toBottom;

        [SerializeField]
        private LogMessage logPrefab;

        private void OnEnable() => this.toBottom.onClick.AddListener(this.ToBottom);

        private void OnDisable() => this.toBottom.onClick.RemoveListener(this.ToBottom);

        public void Add(DebugManager.Log log)
        {
            var logMessage = Instantiate(this.logPrefab, this.content);
            logMessage.Init(log);
        }

        private void ToBottom() => this.scrollRect.verticalNormalizedPosition = 0f;
    }
}
