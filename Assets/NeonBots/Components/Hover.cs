﻿using UnityEngine;

namespace NeonBots.Components
{
    public class Hover : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private float hoverAlt = 3f;

        [SerializeField]
        private float strength = 4000f;

        [SerializeField]
        private float dampening = 10000f;

        private float lastAlt;

        private void FixedUpdate()
        {
            var position = this.transform.position + this.transform.rotation * this.position;
            var ray = new Ray(position, Vector3.down);

            if(Physics.Raycast(ray, out var rayHit, this.hoverAlt, this.layerMask))
            {
                var alt = rayHit.distance;
                var force = this.strength * (this.hoverAlt - alt) + this.dampening * (this.lastAlt - alt);
                force = Mathf.Max(0f, force);
                this.lastAlt = alt;
                this.rigidBody.AddForceAtPosition(this.transform.up * force, position);
            }
            else
            {
                this.lastAlt = this.hoverAlt * 1.1f;
            }
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;
            var position = this.transform.position + this.transform.rotation * this.position;
            var hoverPos = position - new Vector3(0f, this.hoverAlt, 0f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(position, Vector3.one * 0.2f);
            Gizmos.DrawLine(position, hoverPos);

            Gizmos.color = initialColor;
        }
    }
}
