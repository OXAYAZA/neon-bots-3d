using UnityEngine;

namespace NeonBots.Components
{
    public class MapWatcher : MonoBehaviour
    {
        [SerializeField]
        private Watcher watcher;

        private void Update()
        {
            if(this.watcher == default || !this.watcher.isActiveAndEnabled || this.watcher.target == default) return;

            this.transform.position = new(
                this.watcher.target.transform.position.x,
                this.transform.position.y,
                this.watcher.target.transform.position.z
            );
        }
    }
}
