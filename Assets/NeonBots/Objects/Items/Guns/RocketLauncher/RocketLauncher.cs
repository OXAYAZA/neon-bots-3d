using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonBots.Components
{
    public class RocketLauncher : Item
    {
        public Unit owner;

        public Rocket projectilePrefab;

        [SerializeField]
        private float reloadDuration = 0.2f;

        [SerializeField]
        private float shotImpulse = 10f;

        [SerializeField]
        private Transform socketsHinge;

        [SerializeField]
        private List<RocketSocket> sockets;

        [SerializeField]
        private float scanRange = 30f;

        [SerializeField]
        private int scanNumber = 20;

        [SerializeField]
        private LayerMask layerMask;

        private Unit target;

        private Vector3 direction;

        private float reloadTime;

        private void Update()
        {
            this.Reload();
            this.Scan();
            this.Calculate();
            this.Move();
            this.Attack();
        }

        private void Scan()
        {
            this.target = null;

            var objects = new Collider[this.scanNumber];
            Physics.OverlapSphereNonAlloc(this.transform.position, this.scanRange, objects, this.layerMask);

            foreach(var hitCollider in objects)
            {
                if(hitCollider == default || !hitCollider.TryGetComponent<ObjectLink>(out var link) ||
                   link.target == this.owner) continue;

                var target = (Unit)link.target;

                if(target.fraction == this.owner.fraction) continue;

                this.target = target;
                break;
            }
        }

        private void Calculate()
        {
            if(this.target == default)
            {
                this.direction = Vector3.zero;
                return;
            }

            this.direction = this.target.transform.position - this.transform.position;
        }

        private void Move()
        {
            if(this.target == default)
            {
                this.transform.localRotation = Quaternion.identity;
                this.socketsHinge.localRotation = Quaternion.identity;
            }
            else
            {
                var horizontalDirection = this.direction;
                horizontalDirection.y = 0f;
                this.transform.rotation = Quaternion.LookRotation(horizontalDirection);
                this.socketsHinge.localRotation = Quaternion.Euler(-30f, 0f, 0f);
            }
        }

        private void Reload()
        {
            if(this.reloadTime > 0) this.reloadTime -= Time.deltaTime;
            else if(this.reloadTime < 0) this.reloadTime = 0;
        }

        private void Attack()
        {
            if(this.target == default) return;

            if(this.projectilePrefab == default || this.reloadTime > 0) return;

            var readySocket = this.sockets.FirstOrDefault(socket => socket.isReady);

            if(readySocket == default) return;

            readySocket.Fire(this.owner, this.target, this.projectilePrefab, this.shotImpulse);
            this.reloadTime = this.reloadDuration;
        }

        public override void Use() { }
    }
}
