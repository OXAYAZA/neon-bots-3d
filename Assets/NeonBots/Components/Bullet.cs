using UnityEngine;

namespace NeonBots.Components
{
    public class Bullet : Obj
    {
        [Header("Bullet")]
        public float impulse = 25f;

        public float lifeTime = 1f;

        public float damage = 1f;

        private Unit owner;

        [SerializeField]
        private GameObject explosionPrefab;

        private new void Start()
        {
            base.Start();
            this.rigidBody.AddForce(this.transform.forward * this.impulse, ForceMode.Impulse);
        }

        private void Update()
        {
            this.lifeTime -= Time.deltaTime;
            if(this.lifeTime <= 0) this.Death();
        }

        private void OnCollisionEnter(Collision other)
        {
            var unit = other.gameObject.GetComponent<Unit>();
            if(unit is not null && unit != this.owner) unit.hp -= this.damage;

            var bullet = other.gameObject.GetComponent<Bullet>();
            if(bullet is not null && bullet.owner == this.owner) return;
            Debug.Log($"CP0: {other.gameObject.name}");

            this.Death();
        }

        public void Init(Unit owner) => this.owner = owner;

        private void Death()
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
