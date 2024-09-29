using UnityEngine;

namespace NeonBots.Managers
{
    public class InputManager : Manager
    {
        [field: SerializeField]
        public Transform WorldCursor { get; private set; }

        [SerializeField]
        private LayerMask cursorLayers;

        private LocalConfig localConfig;

        private bool TouchControl => MainManager.IsReady && this.localConfig != default &&
            this.localConfig.Get<bool>("touch_control");

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

        private bool resultMainAction;

        private bool tmpMainAction;

        public bool MainAction
        {
            get => this.resultMainAction;
            set => this.tmpMainAction = value;
        }

        private bool resultSecondaryAction;

        private bool tmpSecondaryAction;

        public bool SecondaryAction
        {
            get => this.resultSecondaryAction;
            set => this.tmpSecondaryAction = value;
        }

        private bool resultTertiaryAction;

        private bool tmpTertiaryAction;

        public bool TertiaryAction
        {
            get => this.resultTertiaryAction;
            set => this.tmpTertiaryAction = value;
        }

        private void OnEnable()
        {
            if(MainManager.IsReady) this.OnReady();
            else MainManager.OnReady += this.OnReady;
        }

        private void OnDisable()
        {
            MainManager.OnReady -= this.OnReady;
        }

        private void OnReady()
        {
            this.localConfig = MainManager.GetManager<LocalConfig>();
        }

        private void Update()
        {
            this.resultMovement = Vector2.zero;
            this.resultDirection = Vector2.zero;
            this.resultMainAction = false;
            this.resultSecondaryAction = false;
            this.resultTertiaryAction = false;

            var touchControl = this.TouchControl;
            this.WorldCursor.gameObject.SetActive(!touchControl);

            if(touchControl)
            {
                this.resultMovement = this.tmpMovement;
                this.resultDirection = this.tmpDirection;
                this.resultMainAction = this.tmpMainAction;
                this.resultSecondaryAction = this.tmpSecondaryAction;
                this.resultTertiaryAction = this.tmpTertiaryAction;
            }
            else
            {
                if(Input.GetKey(KeyCode.A)) this.resultMovement += Vector2.left;
                if(Input.GetKey(KeyCode.D)) this.resultMovement += Vector2.right;
                if(Input.GetKey(KeyCode.S)) this.resultMovement += Vector2.down;
                if(Input.GetKey(KeyCode.W)) this.resultMovement += Vector2.up;

                if(Input.GetKey(KeyCode.LeftArrow)) this.resultDirection += Vector2.left;
                if(Input.GetKey(KeyCode.RightArrow)) this.resultDirection += Vector2.right;
                if(Input.GetKey(KeyCode.UpArrow)) this.resultDirection += Vector2.up;
                if(Input.GetKey(KeyCode.DownArrow)) this.resultDirection += Vector2.down;

                if(Input.GetMouseButton(0)) this.resultMainAction = true;
                if(Input.GetMouseButton(1)) this.resultSecondaryAction = true;
                if(Input.GetMouseButton(3)) this.resultTertiaryAction = true;

                this.RefreshWorldCursor();
            }
        }

        private void RefreshWorldCursor()
        {
            this.WorldCursor.gameObject.SetActive(true);

            var camera = MainManager.Instance.mainCamera;

            if(!Physics.Raycast(camera.transform.position, camera.transform.forward, out var hit, 200,
                   this.cursorLayers))
            {
                this.WorldCursor.gameObject.SetActive(false);
                return;
            }

            var ray = camera.ScreenPointToRay(Input.mousePosition);

            var center = hit.point;

            if(!Physics.Raycast(ray.origin, ray.direction, out hit, 200, this.cursorLayers))
            {
                this.WorldCursor.gameObject.SetActive(false);
                return;
            }

            this.WorldCursor.position = hit.point;
            var worldDirection = hit.point - center;
            this.resultDirection = new(worldDirection.x, worldDirection.z);
        }
    }
}
