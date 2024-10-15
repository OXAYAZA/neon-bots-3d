using System;
using UnityEngine;

namespace NeonBots.Components
{
    public class RocketSocket : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem exhaust;

        [SerializeField]
        private AudioClip shotSound;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private float reloadDuration = 3f;

        [NonSerialized]
        public bool isReady;

        private float reloadTime;

        private void Update()
        {
            if(this.reloadTime > 0) this.reloadTime -= Time.deltaTime;
            else if(this.reloadTime < 0) this.reloadTime = 0;
            this.isReady = this.reloadTime <= 0;
        }

        public void Fire(Unit owner, Unit target, Rocket projectilePrefab, float shotImpulse)
        {
            if(projectilePrefab == default || !this.isReady) return;
            var projectile = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
            projectile.Init(owner, target, shotImpulse);
            if(this.exhaust != default) this.exhaust.Play();
            if(this.audioSource && this.shotSound) this.audioSource.PlayOneShot(this.shotSound, 0.05f);
            this.reloadTime = this.reloadDuration;
            this.isReady = false;
        }
    }
}
