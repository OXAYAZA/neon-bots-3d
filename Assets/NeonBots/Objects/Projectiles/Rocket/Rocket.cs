using UnityEngine;

namespace NeonBots.Components
{
    public class Rocket : Projectile
    {
        [Header("Rocket")]
        [SerializeField]
        private float launchTime = 1f;

        [SerializeField]
        private float flyForce = 1f;

        private Unit target;

        public void Init(Unit owner, Unit target, float impulse)
        {
            this.Init(owner, impulse);
            this.target = target;
        }

        protected override void Update()
        {
            base.Update();
            this.rigidBody.AddForce(this.transform.forward * this.flyForce, ForceMode.Force);
            if(this.target == default || this.time < this.launchTime) return;
            this.transform.rotation = Quaternion.LookRotation(this.target.transform.position - this.transform.position);
        }
    }
}
