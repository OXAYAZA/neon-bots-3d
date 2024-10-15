using UnityEngine;

namespace NeonBots.Components
{
    public class Mortar : Item
    {
        public Unit owner;

        public Projectile projectilePrefab;

        [SerializeField]
        private ParticleSystem flame;

        [SerializeField]
        private float reloadDuration = 0.5f;

        [SerializeField]
        private AudioClip shotSound;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private Transform socket;

        [SerializeField]
        private float scanRange = 30f;

        [SerializeField]
        private int scanNumber = 20;

        [SerializeField]
        private LayerMask layerMask;

        private Unit target;

        private Vector3 direction;

        private float shotImpulse = 10f;

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
                this.shotImpulse = 15f;
                return;
            }

            var targetPosition = this.target.transform.position;
            var targetVelocity = this.target.rigidBody.velocity;
            var firstAimPoint = targetPosition + targetVelocity;
            var shotImpulse = this.BallisticVel(firstAimPoint, 45f);
            var timeDistance = 2 * shotImpulse * Mathf.Sin(45f) / Physics.gravity.magnitude;
            var secondAimPoint = targetPosition + targetVelocity * timeDistance;
            this.shotImpulse = this.BallisticVel(secondAimPoint, 45f);

            var direction = secondAimPoint - this.transform.position;
            direction.y = 0f;
            this.direction = direction;
        }

        private void Move()
        {
            if(this.target == default) this.transform.localRotation = Quaternion.identity;
            else this.transform.rotation = Quaternion.LookRotation(this.direction);
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

            var projectile = Instantiate(this.projectilePrefab,
                this.socket.transform.position, this.socket.transform.rotation);
            projectile.Init(this.owner, this.shotImpulse);

            if(this.flame != default) this.flame.Play();
            if(this.audioSource && this.shotSound) this.audioSource.PlayOneShot(this.shotSound, 0.05f);
            this.reloadTime = this.reloadDuration + Random.Range(0f, this.reloadDuration * 0.25f);
        }

        private float BallisticVel(Vector3 target, float angle)
        {
            // get target direction
            var dir = target - this.transform.position;
            // get height difference
            var h = dir.y;
            // retain only the horizontal direction
            dir.y = 0f;
            // get horizontal distance
            var dist = dir.magnitude;
            // convert angle to radians
            var a = angle * Mathf.Deg2Rad;
            // set dir to the elevation angle
            dir.y = dist * Mathf.Tan(a);
            // correct for small height differences
            dist += h / Mathf.Tan(a);
            // calculate the velocity magnitude
            return Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        }

        public override void Use() { }
    }
}
