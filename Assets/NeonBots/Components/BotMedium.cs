using System.Linq;
using UnityEngine;

namespace NeonBots.Components
{
    public class BotMedium : Bot
    {
        private Unit target;

        private Vector3 direction;

        private Vector3 aimPoint;

        private double distance;

        private Gun primaryGun;

        private void Start()
        {
            this.unit = this.GetComponent<Unit>();
            this.primaryGun = this.unit.primarySockets.FirstOrDefault(socket =>
                socket.item != default && socket.item.GetType() == typeof(Gun))?.item as Gun;
        }

        private void Update()
        {
            this.Scan();
            this.Calculate();
            this.Move();
            this.Attack();
            this.Tracking();
        }

        private void Scan()
        {
            if(this.target != default) return;

            this.target = null;

            var objects = new Collider[this.scanNumber];
            Physics.OverlapSphereNonAlloc(this.transform.position, this.scanRange, objects, this.layerMask);

            foreach(var hitCollider in objects)
            {
                if(hitCollider == default || !hitCollider.TryGetComponent<ObjectLink>(out var link) ||
                   link.target == this.unit) continue;

                var target = (Unit)link.target;

                if(target.fraction == this.unit.fraction) continue;

                this.target = target;
                break;
            }
        }

        private void Tracking()
        {
            if(this.target == default) return;
            if((float)this.distance < this.scanRange) return;
            this.target = null;
            this.distance = double.PositiveInfinity;
        }

        private void Calculate()
        {
            this.distance = double.PositiveInfinity;

            if(this.target == default) return;

            this.distance = Vector3.Distance(this.transform.position, this.target.transform.position);

            if(this.primaryGun == default) return;

            var bulletVelocity = this.primaryGun.shotImpulse;
            var timeDistance = (float)this.distance / bulletVelocity;
            var targetPosition = this.target.transform.position;
            var targetVelocity = this.target.rigidBody.velocity;
            this.aimPoint = targetPosition + targetVelocity * timeDistance;
            this.direction = this.aimPoint - this.transform.position;
        }

        private void Move()
        {
            this.unit.Move(Vector3.zero);

            if(this.target == default) return;

            foreach(var socket in this.unit.primarySockets)
                if(socket.rotatable) socket.transform.LookAt(this.aimPoint);

            foreach(var socket in this.unit.secondarySockets)
                if(socket.rotatable) socket.transform.LookAt(this.aimPoint);

            var direction = this.direction;
            direction.y = 0f;
            this.unit.Rotate(direction);

            if(this.distance > this.shotRange * 0.9) this.unit.Move(this.transform.forward);
            else if(this.distance < this.shotRange * 0.8) this.unit.Move(-this.transform.forward);
        }

        private void Attack()
        {
            if(this.target == default) return;
            if(this.distance < this.shotRange) this.unit.UsePrimary();
            this.unit.UseSecondary();
        }
    }
}
