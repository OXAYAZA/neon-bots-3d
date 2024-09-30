﻿using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class Unit : Obj
    {
        [Header("Unit")]
        public float baseHp = 100f;

        public float hp = 100f;

        [SerializeField]
        private float baseSpeed = 3f;

        private Vector3 moveDirection = Vector3.zero;

        private Vector3 lookDirection;

        public List<ItemSocket> primarySockets;

        public List<ItemSocket> secondarySockets;

        protected override void Start()
        {
            this.lookDirection = this.transform.forward;
            this.SetColor(this.color);
        }

        protected virtual void Update()
        {
            if(this.hp <= 0) Destroy(this.gameObject);
        }

        private void FixedUpdate()
        {
            var velocityXZ = Vector3.ProjectOnPlane(this.rigidBody.velocity, Vector3.up).magnitude;
            var targetAccel = (this.baseSpeed - velocityXZ) / Time.deltaTime;
            this.rigidBody.AddForce(this.moveDirection * targetAccel, ForceMode.Acceleration);
            this.transform.rotation = Quaternion.LookRotation(this.lookDirection, Vector3.up);
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

        protected override void SetColor(Color emissionColor)
        {
            foreach(var renderer in this.renderers) renderer.material.SetColor(EmissionColor, emissionColor);
        }
    }
}
