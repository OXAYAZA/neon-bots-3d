using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace NeonBots.UI
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        private float progress;

        public float Progress
        {
            get => this.progress;
            set
            {
                this.progress = value;
                this.Refresh();
            }
        }

        [SerializeField, Tooltip("In seconds")]
        private float animationDuration = 0.25f;

        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private RectTransform bar;

        private CancellationTokenSource cts;

        private void OnEnable() => this.Refresh();

        private void OnRectTransformDimensionsChange() => this.Refresh();

        #if UNITY_EDITOR
        private void OnValidate() => UnityEditor.EditorApplication.delayCall += this._OnValidate;
        
        private void _OnValidate() => this.Refresh();
        #endif

        [ContextMenu("Refresh")]
        private void Refresh()
        {
            if(this.bar == default || this.container == default) return;
            this.bar.sizeDelta = new(this.container.rect.width * this.Progress, this.bar.sizeDelta.y);
        }

        public async UniTask Animate(float val, float duration, CancellationToken ct = default)
        {
            this.ResetAnimation();

            var time = 0f;
            var startVal = this.Progress;
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(this.cts.Token, ct);

            await foreach(var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(linkedCts.Token))
            {
                this.Progress = Mathf.Lerp(startVal, val, time / duration);
                time += Time.deltaTime;

                if(time <= duration) continue;

                this.ResetAnimation();
                this.Progress = val;
            }
        }

        public async UniTask Animate(float val, CancellationToken ct = default) =>
            await this.Animate(val, this.animationDuration, ct);

        private void ResetAnimation()
        {
            this.cts?.Cancel();
            this.cts = new();
        }
    }
}
