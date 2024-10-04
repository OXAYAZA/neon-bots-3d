using UnityEngine;

namespace NeonBots.Components
{
    public class BotMedium : Bot
    {
        private GameObject target;

        private Vector3 direction;

        private Vector3 aimPoint;

        private double distance;

        private void Start()
        {
            this.unit = this.GetComponent<Unit>();
        }

        private void Update()
        {
            if(this.target == default)
            {
                this.Scan();
            }
            else
            {
                this.Calculate();
                this.Move();
                this.Attack();
                this.Tracking();
            }
        }

        private void Scan()
        {
            this.target = null;
            this.distance = double.PositiveInfinity;

            var objects = new Collider[this.scanNumber];
            Physics.OverlapSphereNonAlloc(this.transform.position, this.scanRange, objects, this.layerMask);

            foreach(var hitCollider in objects)
            {
                if(hitCollider == default || !hitCollider.TryGetComponent<ObjectLink>(out var link) ||
                   link.target == this.unit) continue;

                var target = (Unit)link.target;

                if(target.fraction == this.unit.fraction) continue;

                this.target = target.gameObject;
                break;
            }
        }

        private void Tracking()
        {
            if((float)this.distance < this.scanRange) return;
            this.target = null;
            this.distance = double.PositiveInfinity;
        }

        private void Calculate()
        {
            this.distance = Vector3.Distance(this.transform.position, this.target.transform.position);

            if(this.unit.primarySockets.Count <= 0) return;

            var primaryItem = this.unit.primarySockets[0].item;

            if(primaryItem == default && primaryItem.GetType() != typeof(Gun)) return;

            var primaryGun = (Gun)primaryItem;
            var bulletVelocity = primaryGun.projectilePrefab.impulse;
            var timeDistance = (float)this.distance / bulletVelocity;
            var targetPosition = this.target.transform.position;
            var targetVelocity = this.target.GetComponent<Rigidbody>().velocity;
            this.aimPoint = targetPosition + targetVelocity * timeDistance;

            this.direction = this.aimPoint - this.transform.position;
        }

        private void Move()
        {
            foreach(var socket in this.unit.primarySockets)
                if(socket.rotatable) socket.transform.LookAt(this.aimPoint);

            foreach(var socket in this.unit.secondarySockets)
                if(socket.rotatable) socket.transform.LookAt(this.aimPoint);

            this.unit.Rotate(new(
                this.target.transform.position.x - this.transform.position.x,
                0,
                this.target.transform.position.z - this.transform.position.z
            ));

            if(this.distance > this.shotRange * 0.5) this.unit.Move(this.transform.forward);
            else if(this.distance < this.shotRange * 0.25) this.unit.Move(-this.transform.forward);
        }

        private void Attack()
        {
            if(this.distance < this.shotRange) this.unit.Shot();
        }

        private void OnDrawGizmos()
        {
            if(this.target)
            {
                var bulletVelocity = 40f;
                var timeDistance = (float)this.distance / bulletVelocity;
                var targetPosition = this.target.transform.position;
                var targetVelocity = this.target.GetComponent<Rigidbody>().velocity;
                var aimPoint = targetPosition + targetVelocity * timeDistance;

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(this.transform.position, this.target.transform.position - this.transform.position);
                Gizmos.DrawWireSphere(targetPosition, 1.5f);

                Gizmos.color = Color.gray;
                Gizmos.DrawRay(targetPosition, targetVelocity);
                Gizmos.DrawRay(this.transform.position, this.gameObject.GetComponent<Rigidbody>().velocity);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(aimPoint, 1.5f);
                Gizmos.DrawRay(this.transform.position, aimPoint - this.transform.position);
            }
        }
    }
}
