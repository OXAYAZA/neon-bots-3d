using UnityEngine;

public class Death : MonoBehaviour
{
	[SerializeField]
	private bool dieOnCollision = false;

	[SerializeField]
	private float existTime = 10f;

	[SerializeField]
	private Material deathMaterial;

	private GameObject body;
	private Rigidbody rigidBody;
	private new Renderer renderer;

	private void Start ()
	{
		this.body = this.transform.Find( "Body" ).gameObject;
		this.rigidBody = this.GetComponent<Rigidbody>();
		this.renderer = this.body.GetComponent<Renderer>();
	}

	public void Die ()
	{
		this.rigidBody.constraints = RigidbodyConstraints.None;
		this.rigidBody.useGravity = true;

		if ( this.deathMaterial )
			this.renderer.material = this.deathMaterial;

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
