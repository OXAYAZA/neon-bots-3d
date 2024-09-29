using UnityEngine;

namespace NeonBots.Components
{
    public class Hover : MonoBehaviour
    {
        [SerializeField]
        private float hoverAlt = 2f;

        [SerializeField]
        private float takeoffAccel = 100f;

        [SerializeField]
        private Rigidbody rBody;

        [SerializeField]
        private LayerMask layerMask;

        private void FixedUpdate()
        {
            var ray = new Ray(this.transform.position, Vector3.down);
            if(!Physics.Raycast(ray, out var rayHit, this.hoverAlt, this.layerMask)) return;

            var hoverCoef = (this.hoverAlt - rayHit.distance) / this.hoverAlt;
            if(hoverCoef > 0) this.rBody.AddForce(Vector3.up * (this.takeoffAccel * hoverCoef), ForceMode.Acceleration);
        }
    }
}
