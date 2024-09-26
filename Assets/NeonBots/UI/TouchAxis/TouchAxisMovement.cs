using NeonBots.Managers;
using UnityEngine.EventSystems;

namespace NeonBots.UI
{
    public class TouchAxisMovement : TouchAxis
    {
        private InputManager inputManager;

        protected override void OnEnable()
        {
            this.inputManager = MainManager.GetManager<InputManager>();
            base.OnEnable();
        }

        protected override void RefreshValue(PointerEventData eventData)
        {
            base.RefreshValue(eventData);
            this.inputManager.Movement = this.value;
        }

        protected override void ResetPosition()
        {
            base.ResetPosition();
            this.inputManager.Movement = this.value;
        }
    }
}
