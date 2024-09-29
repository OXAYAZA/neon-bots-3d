using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.Components
{
    public class InputController : Controller
    {
        private Unit unit;

        private InputManager inputManager;

        public void Init(Unit unit = default)
        {
            this.unit = unit == default ? this.GetComponent<Unit>() : unit;

            if(this.unit == default) Debug.LogError($"[{this.name}][{nameof(InputController)}] Init failed.");

            this.inputManager = MainManager.GetManager<InputManager>();
        }

        private void Update()
        {
            if(this.inputManager is null) return;

            if(this.unit)
            {
                var movementVector = new Vector3(this.inputManager.Movement.x, 0, this.inputManager.Movement.y);
                var directionVector = new Vector3(this.inputManager.Direction.x, 0, this.inputManager.Direction.y);

                this.unit.Move(movementVector);
                this.unit.Rotate(directionVector);

                foreach(var socket in this.unit.primarySockets)
                    if(socket.rotatable) socket.transform.LookAt(this.inputManager.WorldCursor);

                foreach(var socket in this.unit.secondarySockets)
                    if(socket.rotatable) socket.transform.LookAt(this.inputManager.WorldCursor);

                if(this.inputManager.MainAction) this.unit.Shot();
            }
        }
    }
}
