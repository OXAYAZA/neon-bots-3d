using UnityEngine;

namespace NeonBots.Managers
{
    public class InputManager : Manager
    {
        private Vector2 resultMovement;

        private Vector2 tmpMovement;

        public Vector2 Movement
        {
            get => this.resultMovement;
            set => this.tmpMovement = value;
        }

        private Vector2 resultDirection;

        private Vector2 tmpDirection;

        public Vector2 Direction
        {
            get => this.resultDirection;
            set => this.tmpDirection = value;
        }

        private bool resultShot;

        private bool tmpShot;

        public bool Shot
        {
            get => this.resultShot;
            set => this.tmpShot = value;
        }

        private void Update()
        {
            this.resultMovement = Vector2.zero;
            this.resultDirection = Vector2.zero;
            this.resultShot = false;

            this.resultMovement = this.tmpMovement;
            this.resultDirection = this.tmpDirection;
            this.resultShot = this.tmpShot;

            if(Input.GetKey(KeyCode.A)) this.resultMovement += Vector2.left;
            if(Input.GetKey(KeyCode.D)) this.resultMovement += Vector2.right;
            if(Input.GetKey(KeyCode.S)) this.resultMovement += Vector2.down;
            if(Input.GetKey(KeyCode.W)) this.resultMovement += Vector2.up;

            if(Input.GetKey(KeyCode.LeftArrow)) this.resultDirection += Vector2.left;
            if(Input.GetKey(KeyCode.RightArrow)) this.resultDirection += Vector2.right;
            if(Input.GetKey(KeyCode.UpArrow)) this.resultDirection += Vector2.up;
            if(Input.GetKey(KeyCode.DownArrow)) this.resultDirection += Vector2.down;

            if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) this.resultShot = true;

            // TODO: Cursor control.
            // if(this.camera)
            // {
            //     this._cursorPos = this.cameraController.cursor;
            //
            //     if(this._cursorPos != this.camera.transform.position)
            //     {
            //         var direction = new Vector3(this._cursorPos.x - this.hero.transform.position.x, 0,
            //             this._cursorPos.z - this.hero.transform.position.z);
            //         this.hero.Rotate(direction);
            //     }
            // }
        }
    }
}
