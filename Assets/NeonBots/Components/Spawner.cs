using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class Spawner : Unit
    {
        [Header("Spawner")][SerializeField]
        private Obj spawnedObject;

        [SerializeField]
        private float spawnPeriod = 5f;

        private float spawnTimer;

        private List<GameObject> triggers;

        protected override void Start()
        {
            base.Start();
            this.triggers = new();
            this.spawnTimer = this.spawnPeriod;
        }

        private void SpawnObject()
        {
            var initialTransform = this.transform;
            var obj = Instantiate(this.spawnedObject, initialTransform.position, initialTransform.rotation);

            obj.fraction = this.fraction;
            obj.color = this.color;
        }

        private void Update()
        {
            if(this.spawnTimer <= 0)
            {
                if(this.triggers.Count <= 0) this.SpawnObject();
                this.spawnTimer = this.spawnPeriod;
            }
            else
            {
                this.spawnTimer -= Time.deltaTime;
            }

            if(this.triggers.Count != 0)
            {
                var busyColor = new Color(1, 1, 1, 0.5f);
                this.renderer.material.SetColor("_Color", busyColor);
                this.renderer.material.SetColor("_EmissionColor", busyColor);
            }
            else
            {
                this.renderer.material.SetColor("_Color", this.color);
                this.renderer.material.SetColor("_EmissionColor", this.color);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 3) this.triggers.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            this.triggers.Remove(other.gameObject);
        }
    }
}
