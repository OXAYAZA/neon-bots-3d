using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Color color = new Color( 200, 200, 200 );
	public float force = 400f;
	public Vector3 initialVelocity = new Vector3( 0, 0, 0 );
	public float lifeTime = .5f;
	public float damage = 10f;

	private Renderer _renderer;
	private Rigidbody _rigidbody;

	private void Start ()
	{
		this._rigidbody = this.GetComponent<Rigidbody>();
		this._renderer = this.GetComponent<Renderer>();
		this.transform.Rotate( 90f, 0, 0 );
		this._rigidbody.velocity = this.initialVelocity;
		this._rigidbody.AddForce( this.transform.up * this.force, ForceMode.Impulse );

		this._renderer.material.SetColor( "_Color", this.color );
		this._renderer.material.SetColor( "_EmissionColor", this.color );
	}

	private void Update ()
	{
		this.lifeTime -= Time.deltaTime;

		if ( this.lifeTime <= 0 )
		{
			Destroy( this.gameObject );
		}
	}

	private void OnCollisionEnter( Collision other )
	{
		var unit = other.gameObject.GetComponent<Unit>();

		if ( unit )
		{
			unit.hp -= this.damage;
		}
	}
}
