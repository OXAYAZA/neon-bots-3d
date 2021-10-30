using UnityEngine;

public class Bullet : RootObject
{
	public float force = 400f;
	public Vector3 initialVelocity = new Vector3( 0, 0, 0 );
	public float lifeTime = .5f;
	public float damage = 10f;

	private new void Start ()
	{
		base.Start();

		this.rigidBody.velocity = this.initialVelocity;
		this.rigidBody.AddForce( this.transform.forward * this.force, ForceMode.Impulse );

		this.renderer.material.SetColor( "_Color", this.data.color );
		this.renderer.material.SetColor( "_EmissionColor", this.data.color );
	}

	private void Update ()
	{
		this.lifeTime -= Time.deltaTime;

		if ( this.lifeTime <= 0 )
			Destroy( this.gameObject );
	}

	private void OnCollisionEnter( Collision other )
	{
		var unit = other.gameObject.GetComponent<Unit>();

		if ( unit )
			unit.hp -= this.damage;
	}
}
