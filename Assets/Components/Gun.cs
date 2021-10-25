using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField]
	private GameObject bullet;

	[SerializeField]
	private float shotReload = 0.5f;

	[SerializeField]
	private AudioClip shotSound;

	private Unit _unit;
	private Rigidbody _rigidBody;
	private AudioSource _audioSource;
	private float _shotReloading = 0;
	private List<Transform> _sockets = new List<Transform>();

	private void Start ()
	{
		this._unit = this.gameObject.GetComponent<Unit>();
		this._rigidBody = this.gameObject.GetComponent<Rigidbody>();
		this._audioSource = this.GetComponent<AudioSource>();

		var children = this.transform.childCount;
		for ( var i = 0; i < children; ++i )
		{
			var child = this.transform.GetChild( i );
			if ( child.name == "Socket" ) this._sockets.Add( child );
		}
	}

	private void Update ()
	{
		this.Reload();
	}

	private void Reload ()
	{
		if ( this._shotReloading > 0 )
			this._shotReloading -= Time.deltaTime;
		else if ( this._shotReloading < 0 )
			this._shotReloading = 0;
	}

	public void Shot ()
	{
		if ( this.bullet != null && this._shotReloading <= 0 )
		{
			foreach ( var socket in this._sockets )
			{
				var iniTrans = socket.transform;
				var tmp = Instantiate( this.bullet, iniTrans.position, iniTrans.rotation );
				var tmpBullet = tmp.GetComponent<Bullet>();

				if ( tmpBullet )
				{
					tmpBullet.color = this._unit.color;
					tmpBullet.initialVelocity = this._rigidBody.velocity;
				}
			}

			if ( this._audioSource && this.shotSound )
				this._audioSource.PlayOneShot( this.shotSound, 0.05f );

			this._shotReloading = this.shotReload;
		}
	}
}
