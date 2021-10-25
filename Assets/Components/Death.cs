using UnityEngine;

public class Death : MonoBehaviour
{
	[SerializeField]
	private bool dieOnCollision;

	[SerializeField]
	private float existTime = 10f;

	[SerializeField]
	private Material deathMaterial;

	private Renderer _renderer;
	private Rigidbody _rigidbody;

	private void Start ()
	{
		this._renderer = this.GetComponent<Renderer>();
		this._rigidbody = this.GetComponent<Rigidbody>();
	}

	public void Die ()
	{
		this._rigidbody.constraints = RigidbodyConstraints.None;
		this._rigidbody.useGravity = true;

		if ( this.deathMaterial )
			this._renderer.material = this.deathMaterial;

		foreach ( MonoBehaviour script in this.gameObject.GetComponents<MonoBehaviour>() )
		{
			var scriptName = script.GetType().Name;

			if ( scriptName != "Death" )
				Destroy( script );
		}

		this.Invoke( nameof( this.RemoveGameObject ), this.existTime );
	}

	private void RemoveGameObject ()
	{
		Destroy( this.gameObject );
	}

	private void OnCollisionEnter( Collision other )
	{
		if ( this.dieOnCollision )
			this.Die();
	}
}
