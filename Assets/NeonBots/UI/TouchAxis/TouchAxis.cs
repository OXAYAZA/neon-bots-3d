using System;
using NeonBots.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NeonBots.UI
{
    public class TouchAxis : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private RectTransform area;

        [SerializeField]
        private RectTransform handle;

        [SerializeField]
        private TMP_Text data;

        [NonSerialized]
        public Vector2 value = Vector2.zero;

        private UIManager uiManager;

        protected virtual void OnEnable()
        {
            this.uiManager = MainManager.GetManager<UIManager>();
            this.ResetPosition();
            this.RefreshData();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.RefreshValue(eventData);
            this.RefreshData();
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.RefreshValue(eventData);
            this.RefreshData();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.ResetPosition();
            this.RefreshData();
        }

        // Area and handle pivot must be 0.5 0.5
        // Handle anchors must be 0.5 0.5
        protected virtual void RefreshValue(PointerEventData eventData)
        {
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.area, eventData.position,
                eventData.pressEventCamera, out var position)) return;
            var areaHalfSize = this.area.sizeDelta / 2;
            position = new(
                Mathf.Clamp(position.x, -areaHalfSize.x, areaHalfSize.x),
                Mathf.Clamp(position.y, -areaHalfSize.y, areaHalfSize.y)
            );
            this.handle.anchoredPosition = position;
            this.value = position / areaHalfSize;
        }

        protected virtual void ResetPosition()
        {
            this.value = Vector2.zero;
            this.handle.anchoredPosition = Vector2.zero;
        }

        private void RefreshData() =>
            this.data.text = $"X: {this.value.x} Y: {this.value.y}\nM: {this.value.magnitude}";
    }
}
