﻿using NeonBots.Managers;
using UnityEngine.EventSystems;

namespace NeonBots.UI
{
    public class TouchAxisDirection : TouchAxis
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
            this.inputManager.Direction = this.value;
            this.inputManager.MainAction = true;
        }

        protected override void ResetPosition()
        {
            base.ResetPosition();
            this.inputManager.Direction = this.value;
            this.inputManager.MainAction = false;
        }
    }
}
