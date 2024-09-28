using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.Components
{
    public class InputController : MonoBehaviour
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

                if(this.inputManager.MainAction) this.unit.Shot();
            }
        }
    }
}
