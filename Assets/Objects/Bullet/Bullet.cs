using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Color color = new Color( 200, 200, 200 );
	public float force = 400f;
	public Vector3 initialVelocity = new Vector3( 0, 0, 0 );
	public float lifeTime = .5f;
	public float damage = 10f;

	private RootObject ro;

	private void Start ()
	{
		this.ro = this.gameObject.GetComponent<RootObject>();

		this.ro.rigidbody.velocity = this.initialVelocity;
		this.ro.rigidbody.AddForce( this.transform.forward * this.force, ForceMode.Impulse );

		this.ro.renderer.material.SetColor( "_Color", this.color );
		this.ro.renderer.material.SetColor( "_EmissionColor", this.color );
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
