using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Bullet bulletPrefab;

    [SerializeField]
    private float reloadDuration = 0.5f;

    [SerializeField]
    private AudioClip shotSound;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private List<Transform> sockets;

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

    public void Shot()
    {
        if(this.bulletPrefab != null && this.reloadTime <= 0)
        {
            foreach(var socket in this.sockets)
            {
                var iniTrans = socket.transform;
                var bullet = Instantiate(this.bulletPrefab, iniTrans.position, iniTrans.rotation);
            }

            if(this.audioSource && this.shotSound) this.audioSource.PlayOneShot(this.shotSound, 0.05f);
            this.reloadTime = this.reloadDuration;
        }
    }
}
