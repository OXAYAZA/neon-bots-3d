using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class MobileSpawner : Item
    {
        public Unit owner;

        [SerializeField]
        private Unit unitPrefab;

        [SerializeField]
        private float reloadDuration = 2f;

        [SerializeField]
        private Transform socket;

        [SerializeField]
        private List<Spawner.Mod> mods;

        private float reloadTime;

        private void Update()
        {
            this.Reload();
        }

        private void Reload()
        {
            if(this.reloadTime > 0) this.reloadTime -= Time.deltaTime;
            else if(this.reloadTime < 0) this.reloadTime = 0;
        }

        public override void Use()
        {
            if(this.unitPrefab == default || this.reloadTime > 0) return;

            var unit = Instantiate(this.unitPrefab, this.socket.transform.position, this.socket.transform.rotation);
            this.Modify(unit);
            this.reloadTime = this.reloadDuration;
        }

        private void Modify(Obj obj)
        {
            obj.fraction = this.owner.fraction;
            obj.color = this.owner.color;

            var isBot = obj.TryGetComponent<Bot>(out var bot);
            var isUnit = obj.GetType() == typeof(Unit);
            var unit = isUnit ? (Unit)obj : null;

            foreach(var mod in this.mods)
            {
                switch(mod.type)
                {
                    case Spawner.ModType.ScanRange:
                        if(isBot) bot.scanRange = mod.value;
                        break;
                    case Spawner.ModType.ScanNumber:
                        if(isBot) bot.scanNumber = (int)mod.value;
                        break;
                    case Spawner.ModType.ShotRange:
                        if(isBot) bot.shotRange = mod.value;
                        break;
                    case Spawner.ModType.Hp:
                        if(isUnit) unit.hp = unit.baseHp = mod.value;
                        break;
                    case Spawner.ModType.Speed:
                        if(isUnit) unit.baseSpeed = mod.value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
