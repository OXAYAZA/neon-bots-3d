using UnityEngine;

namespace NeonBots.Components
{
    public class Projectile : Obj
    {
        [Header("Projectile")]
        public float impulse = 25f;

        public float lifeTime = 1f;

        public float damage = 1f;

        protected Unit owner;

        [SerializeField]
        private GameObject explosionPrefab;

        protected override void Start()
        {
            base.Start();
            this.rigidBody.AddForce(this.transform.forward * this.impulse, ForceMode.Impulse);

            if(this.owner != default)
                foreach(var pCollider in this.colliders)
                    foreach(var oCollider in this.owner.colliders)
                        Physics.IgnoreCollision(pCollider, oCollider, true);
        }

        private void Update()
        {
            this.lifeTime -= Time.deltaTime;
            if(this.lifeTime <= 0) this.Death();
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            var unit = other.gameObject.GetComponent<Unit>();
            if(unit is not null) unit.hp -= this.damage;

            var projectile = other.gameObject.GetComponent<Projectile>();
            if(projectile is not null && projectile.owner == this.owner) return;

            this.Death();
        }

        public void Init(Unit owner, float impulse)
        {
            this.owner = owner;
            this.impulse = impulse;
        }

        protected void Death()
        {
            Destroy(this.gameObject);

            if(this.explosionPrefab != default)
            {
                var trm = this.transform;
                Instantiate(this.explosionPrefab, trm.position, trm.rotation);
            }
        }
    }
}
