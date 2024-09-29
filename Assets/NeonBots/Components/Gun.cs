using UnityEngine;

namespace NeonBots.Components
{
    public class Gun : Item
    {
        public Unit owner;

        public Bullet projectilePrefab;

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
            if(this.projectilePrefab != default && this.reloadTime <= 0)
            {
                var projectile = Instantiate(this.projectilePrefab,
                    this.socket.transform.position, this.socket.transform.rotation);
                projectile.Init(this.owner);

                if(this.flame != default) this.flame.Play();
                if(this.audioSource && this.shotSound) this.audioSource.PlayOneShot(this.shotSound, 0.05f);
                this.reloadTime = this.reloadDuration;
            }
        }
    }
}
