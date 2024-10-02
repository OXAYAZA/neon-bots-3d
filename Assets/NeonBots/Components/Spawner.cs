using UnityEngine;

namespace NeonBots.Components
{
    public class Spawner : Unit
    {
        [Header("Spawner")]
        [SerializeField]
        private Obj spawnedObject;

        [SerializeField]
        private float spawnPeriod = 5f;

        [SerializeField]
        private float restrictedRadius = 1f;

        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private Color disabledColor = new(0.8f, 0.8f, 0.8f, 0.5f);

        [SerializeField]
        private SphereCollider spawnCollider;

        private float spawnTimer;

        private bool occupied;

        protected override void Start()
        {
            base.Start();
            this.SetColor(this.color, this.color);
            this.spawnTimer = this.spawnPeriod;
        }

        private void SpawnObject()
        {
            var initialTransform = this.transform;
            var obj = Instantiate(this.spawnedObject, initialTransform.position, initialTransform.rotation);
            obj.fraction = this.fraction;
            obj.color = this.color;
        }

        protected override void Update()
        {
            base.Update();
            this.Scan();

            if(this.occupied)
            {
                this.spawnCollider.enabled = false;
                this.SetColor(this.disabledColor, Color.black);
                return;
            }

            this.spawnCollider.enabled = true;
            this.SetColor(this.color, this.color);

            if(this.spawnTimer <= 0)
            {
                if(!this.occupied) this.SpawnObject();
                this.spawnTimer = this.spawnPeriod;
            }
            else
            {
                this.spawnTimer -= Time.deltaTime;
            }
        }

        private void Scan()
        {
            this.occupied = false;

            var objects = new Collider[10];
            Physics.OverlapSphereNonAlloc(this.transform.position, this.restrictedRadius, objects, this.layerMask);

            foreach(var collider in objects)
            {
                if(collider == default || collider == this.spawnCollider) continue;
                this.occupied = true;
                break;
            }
        }
    }
}
