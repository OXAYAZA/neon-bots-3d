using UnityEngine;

public class Death : MonoBehaviour
{
	[SerializeField]
	private bool dieOnCollision;

	[SerializeField]
	private float existTime = 10f;

	[SerializeField]
	private Material deathMaterial;

	private RootObject ro;

	private void Start ()
	{
		this.ro = this.GetComponent<RootObject>();
	}

	public void Die ()
	{
		this.ro.rigidbody.constraints = RigidbodyConstraints.None;
		this.ro.rigidbody.useGravity = true;

		if ( this.deathMaterial )
			this.ro.renderer.material = this.deathMaterial;

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
