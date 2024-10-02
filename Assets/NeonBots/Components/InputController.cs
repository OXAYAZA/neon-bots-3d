using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.Components
{
    public class InputController : Controller
    {
        private Unit unit;

        private InputManager inputManager;

        private LocalConfig localConfig;

        public void Init(Unit unit = default)
        {
            this.unit = unit == default ? this.GetComponent<Unit>() : unit;

            if(this.unit == default) Debug.LogError($"[{this.name}][{nameof(InputController)}] Init failed.");

            this.inputManager = MainManager.GetManager<InputManager>();
            this.localConfig = MainManager.GetManager<LocalConfig>();
        }

        private void Update()
        {
            if(this.inputManager is null || MainManager.GamePaused) return;

            if(this.unit)
            {
                var movementVector = new Vector3(this.inputManager.Movement.x, 0f, this.inputManager.Movement.y);
                var directionVector = new Vector3(this.inputManager.Direction.x, 0f, this.inputManager.Direction.y);

                this.unit.Move(movementVector);
                this.unit.Rotate(directionVector);

                if(!this.localConfig.Get<bool>("touch_control"))
                {
                    foreach(var socket in this.unit.primarySockets)
                        if(socket.rotatable) socket.transform.LookAt(this.inputManager.WorldCursor);

                    foreach(var socket in this.unit.secondarySockets)
                        if(socket.rotatable) socket.transform.LookAt(this.inputManager.WorldCursor);
                }
                else
                {
                    foreach(var socket in this.unit.primarySockets)
                        if(socket.rotatable) socket.transform.localRotation = Quaternion.identity;

                    foreach(var socket in this.unit.secondarySockets)
                        if(socket.rotatable) socket.transform.localRotation = Quaternion.identity;
                }

                if(this.inputManager.MainAction) this.unit.Shot();
            }
        }
    }
}
