using System;
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

        [NonSerialized]
        public Vector2 value = Vector2.zero;

        protected virtual void OnEnable() => this.ResetPosition();

        public void OnPointerDown(PointerEventData eventData) => this.RefreshValue(eventData);

        public void OnDrag(PointerEventData eventData) => this.RefreshValue(eventData);

        public void OnPointerUp(PointerEventData eventData) => this.ResetPosition();

        // Area and handle pivot must be 0.5 0.5
        // Handle anchors must be 0.5 0.5
        protected virtual void RefreshValue(PointerEventData eventData)
        {
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.area, eventData.position,
                eventData.pressEventCamera, out var position)) return;
            var areaHalfSize = this.area.sizeDelta / 2;
            position = Vector2.ClampMagnitude(position, areaHalfSize.x);
            this.handle.anchoredPosition = position;
            this.value = position / areaHalfSize;
        }

        protected virtual void ResetPosition()
        {
            this.value = Vector2.zero;
            this.handle.anchoredPosition = Vector2.zero;
        }
    }
}
