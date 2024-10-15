using UnityEngine;

namespace NeonBots.Components
{
    public class ExplosiveProjectile : Projectile
    {
        [Header("Explosive Projectile")]
        [SerializeField]
        private float explosionRadius = 2f;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private int scanNumber = 10;

        protected override void OnCollisionEnter(Collision other)
        {
            var objects = new Collider[this.scanNumber];
            Physics.OverlapSphereNonAlloc(this.transform.position, this.explosionRadius, objects, this.layerMask);
            
            foreach(var hitCollider in objects)
            {
                if(hitCollider == default || !hitCollider.TryGetComponent<ObjectLink>(out var link) ||
                   link.target == this.owner) continue;

                var target = (Unit)link.target;

                if(target.fraction == this.owner.fraction) continue;

                target.hp -= this.damage;
            }
            
            this.Death();
        }

        protected void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);

            Gizmos.color = initialColor;
        }
    }
}
