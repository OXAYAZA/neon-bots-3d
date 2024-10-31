using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.Components
{
    public class WorldCursor : MonoBehaviour
    {
        // TODO: Get watcher from camera.
        [SerializeField]
        private Watcher watcher;

        [SerializeField]
        private LayerMask cursorLayers;

        public Vector3 Position { get; private set; }

        public Vector3 Center { get; private set; }

        public Vector3 Direction { get; private set; }

        public Object Target { get; private set; }

        private void Update()
        {
            var camera = MainManager.Camera;

            if(!Physics.Raycast(camera.transform.position, camera.transform.forward, out var hit, 200,
                   this.cursorLayers)) return;

            this.Center = this.watcher.target != default ? this.watcher.target.transform.position : hit.point;
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if(!Physics.Raycast(ray.origin, ray.direction, out hit, 200, this.cursorLayers)) return;

            this.Target = hit.collider.gameObject;
            this.Position = this.transform.position = this.watcher.target != default
                ? new(hit.point.x, this.watcher.target.transform.position.y, hit.point.z) : hit.point;
            this.Direction = this.Position - this.Center;

            if(!hit.collider.TryGetComponent<ObjectLink>(out var link) || link.target == default) return;

            this.Target = link.target;
            this.Position = this.transform.position = hit.point;
            this.Direction = this.Position - this.Center;
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(this.Center, 0.5f);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(this.Position, 0.5f);

            Gizmos.color = initialColor;
        }
    }
}
