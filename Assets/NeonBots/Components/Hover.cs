using UnityEngine;

namespace NeonBots.Components
{
    public class Hover : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rBody;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private float maxHoverAlt = 3f;

        [SerializeField]
        private float minHoverAlt = 2f;

        [SerializeField]
        private float maxForce = 20f;

        [SerializeField]
        private float minForce = 9.81f;

        private void FixedUpdate()
        {
            var ray = new Ray(this.transform.position, Vector3.down);

            var altitude = Physics.Raycast(ray, out var rayHit, this.maxHoverAlt, this.layerMask)
                ? rayHit.distance : this.maxHoverAlt + 1f;
            var coeff = altitude > this.minHoverAlt ? 0f : (this.minHoverAlt - altitude) / this.minHoverAlt;
            var force = 0f;

            if(altitude < this.maxHoverAlt && altitude > this.minHoverAlt && this.rBody.velocity.y < 0f)
                force = this.minForce;

            if(altitude < this.minHoverAlt)
                force = (this.maxForce - this.minForce) * coeff + this.minForce;

            this.rBody.AddForce(Vector3.up * force, ForceMode.Acceleration);
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;

            var position = this.transform.position;
            var hoverDiff = this.maxHoverAlt - this.minHoverAlt;
            var minHoverPos = position - new Vector3(0f, this.minHoverAlt, 0f);
            var maxHoverPos = minHoverPos - new Vector3(0f, hoverDiff, 0f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(position, minHoverPos);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(minHoverPos, maxHoverPos);

            Gizmos.color = initialColor;
        }
    }
}
