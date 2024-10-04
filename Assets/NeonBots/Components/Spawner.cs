using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class Spawner : Unit
    {
        public enum ModType
        {
            ScanRange,
            ScanNumber,
            ShotRange,
            Hp,
            Speed
        }

        [Serializable]
        public class Mod
        {
            public ModType type;

            public float value;
        }

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

        [SerializeField]
        private List<Mod> mods;

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
            this.Modify(obj);
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

        private void Modify(Obj obj)
        {
            obj.fraction = this.fraction;
            obj.color = this.color;

            var isBot = obj.TryGetComponent<Bot>(out var bot);
            var isUnit = obj.GetType() == typeof(Unit);
            var unit = isUnit ? (Unit)obj : null;

            foreach(var mod in this.mods)
            {
                switch(mod.type)
                {
                    case ModType.ScanRange:
                        if(isBot) bot.scanRange = mod.value;
                        break;
                    case ModType.ScanNumber:
                        if(isBot) bot.scanNumber = (int)mod.value;
                        break;
                    case ModType.ShotRange:
                        if(isBot) bot.shotRange = mod.value;
                        break;
                    case ModType.Hp:
                        if(isUnit) unit.hp = unit.baseHp = mod.value;
                        break;
                    case ModType.Speed:
                        if(isUnit) unit.baseSpeed = mod.value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
