﻿using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class Unit : Obj
    {
        [Header("Unit")]
        public float maxHp = 100f;

        public float hp = 100f;

        [SerializeField]
        private float acceleration = 500f;

        [SerializeField]
        private float torque = 50f;

        private Vector3 movementDirection = Vector3.zero;

        private Vector3 rotatingDirection = Vector3.zero;

        private Vector3 expectedMovementDirection = Vector3.zero;

        private Vector3 expectedRotatingDirection = Vector3.zero;

        public List<ItemSocket> primarySockets;

        public List<ItemSocket> secondarySockets;

        protected override void Start() => this.SetColor();

        private void Update()
        {
            this.movementDirection = Vector3.zero;
            this.rotatingDirection = Vector3.zero;

            if(this.expectedMovementDirection != Vector3.zero)
            {
                this.movementDirection = this.expectedMovementDirection;
                this.expectedMovementDirection = Vector3.zero;
            }

            if(this.expectedRotatingDirection != Vector3.zero)
            {
                this.rotatingDirection = this.expectedRotatingDirection;
                this.expectedRotatingDirection = Vector3.zero;
            }

            if(this.hp <= 0) Destroy(this.gameObject);
        }

        private void FixedUpdate()
        {
            this.rigidBody.AddForce(this.movementDirection * this.acceleration, ForceMode.Acceleration);
            this.rigidBody.AddTorque(this.rotatingDirection * this.torque, ForceMode.Acceleration);
        }

        public void ResetValues()
        {
            this.hp = this.maxHp;
        }

        public void Move(Vector3 direction)
        {
            this.expectedMovementDirection += direction;

            if(this.expectedMovementDirection.magnitude > Vector3.one.magnitude)
                this.expectedMovementDirection.Normalize();
        }

        public void Rotate(Vector3 direction)
        {
            var angle = Vector3.SignedAngle(this.transform.forward, direction, this.transform.up);

            if(angle < 0)
                this.expectedRotatingDirection -= this.transform.up;
            else if(angle > 0)
                this.expectedRotatingDirection += this.transform.up;

            if(this.expectedRotatingDirection.magnitude > Vector3.one.magnitude)
                this.expectedRotatingDirection.Normalize();
        }

        public void Shot()
        {
            foreach(var item in this.primarySockets) item.Use();
        }
    }
}
