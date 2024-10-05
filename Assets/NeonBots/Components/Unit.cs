using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class Unit : Obj
    {
        [Header("Unit")]
        public float baseHp = 100f;

        public float hp = 100f;

        public float baseSpeed = 3f;

        [SerializeField]
        private float horizontalDrag = 200f;

        [SerializeField]
        private ParticleSystem explosionPrefab;

        [Space]
        public List<ItemSocket> primarySockets;

        [Space]
        public List<ItemSocket> secondarySockets;

        private Vector3 moveDirection = Vector3.zero;

        private Vector3 lookDirection;

        protected override void Start()
        {
            this.lookDirection = this.transform.forward;
            this.SetColor(this.color);
        }

        protected virtual void Update()
        {
            if(this.hp <= 0) this.Death();
        }

        private void FixedUpdate()
        {
            var velocityXZ = Vector3.ProjectOnPlane(this.rigidBody.velocity, Vector3.up).magnitude;
            var targetAccel = (this.baseSpeed - velocityXZ) / Time.deltaTime;
            this.rigidBody.AddForce(this.moveDirection * targetAccel, ForceMode.Acceleration);
            this.transform.rotation = Quaternion.LookRotation(this.lookDirection, Vector3.up);

            var velocityProj = Vector3.ProjectOnPlane(this.rigidBody.velocity, Vector3.up);
            this.rigidBody.AddForce(-velocityProj.normalized * this.horizontalDrag, ForceMode.Acceleration);
        }

        public void ResetValues()
        {
            this.hp = this.baseHp;
        }

        public void Move(Vector3 direction)
        {
            this.moveDirection = direction;
        }

        public void Rotate(Vector3 direction)
        {
            if(direction == Vector3.zero) return;
            this.lookDirection = direction;
        }

        public void Shot()
        {
            foreach(var item in this.primarySockets) item.Use();
        }

        protected override void SetColor(Color color)
        {
            if(this.mapFigure != default) this.mapFigure.color = color;
            foreach(var renderer in this.renderers) renderer.material.SetColor(EmissionColor, color);
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;
            var position = this.transform.position;

            // Move direction
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(position, this.moveDirection.normalized * 3f);

            // Velocity vector
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position, this.rigidBody.velocity);

            // Horizontal drag direction
            var velocityProj = Vector3.ProjectOnPlane(this.rigidBody.velocity, Vector3.up);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(position, -velocityProj.normalized * 3f);

            Gizmos.color = initialColor;
        }

        private void Death()
        {
            Destroy(this.gameObject);

            if(this.explosionPrefab != default)
            {
                var trm = this.transform;
                var explosion = Instantiate(this.explosionPrefab, trm.position, trm.rotation);
                var main = explosion.main;
                main.startColor = this.color;
            }
        }
    }
}
