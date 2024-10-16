using UnityEngine;

namespace NeonBots.Components
{
    public class BotEasy : Bot
    {
        [Header("Bot Easy")]
        [SerializeField]
        private float battleDistanceMin = 8f;

        [SerializeField]
        private float battleDistanceMax = 10f;

        private GameObject target;

        private Vector3 direction;

        private double distance;

        private void Start()
        {
            this.unit = this.GetComponent<Unit>();
        }

        private void Update()
        {
            this.Scan();

            if(!this.target)
            {
                this.unit.Move(Vector3.zero);
            }
            else
            {
                this.Calculate();
                this.Move();
                this.Attack();
            }
        }

        private void Scan()
        {
            this.target = null;
            this.distance = double.PositiveInfinity;
            this.direction = Vector3.zero;

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

        private void Calculate()
        {
            this.direction = new(
                this.target.transform.position.x - this.transform.position.x,
                0,
                this.target.transform.position.z - this.transform.position.z
            );

            this.distance = Vector3.Distance(this.transform.position, this.target.transform.position);
        }

        private void Move()
        {
            foreach(var socket in this.unit.primarySockets)
                if(socket.rotatable) socket.transform.LookAt(this.target.transform);

            foreach(var socket in this.unit.secondarySockets)
                if(socket.rotatable) socket.transform.LookAt(this.target.transform);

            this.unit.Rotate(this.direction);

            if(this.distance > this.battleDistanceMax) this.unit.Move(this.transform.forward);
            else if(this.distance < this.battleDistanceMin) this.unit.Move(-this.transform.forward);
            else this.unit.Move(-this.transform.right);
        }

        private void Attack()
        {
            if(this.distance < this.shotRange) this.unit.UsePrimary();
        }
    }
}
