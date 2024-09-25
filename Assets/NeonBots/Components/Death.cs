using UnityEngine;

public class Death : MonoBehaviour
{
	[SerializeField]
	private bool dieOnCollision = false;

	[SerializeField]
	private float existTime = 10f;

	[SerializeField]
	private Material deathMaterial;

	[SerializeField]
	private GameObject explosionPrefab;

	private ObjectData data;
	private GameObject body;
	private Rigidbody rigidBody;
	private new Renderer renderer;
	private bool state = true;

	private void Start ()
	{
		this.body = this.transform.Find( "Body" ).gameObject;
		this.data = this.GetComponent<ObjectData>();
		this.rigidBody = this.GetComponent<Rigidbody>();
		this.renderer = this.body.GetComponent<Renderer>();
	}

	public void Die ()
	{
		this.state = false;
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

		if ( this.explosionPrefab )
		{
			var iniTrans = this.transform;
			var tmpObject = Instantiate( this.explosionPrefab, iniTrans.position, iniTrans.rotation );
			ParticleSystem.MainModule main = tmpObject.GetComponent<ParticleSystem>().main;
			main.startColor = this.data.color;
		}

		this.Invoke( nameof( this.RemoveGameObject ), this.existTime );
	}

	private void RemoveGameObject ()
	{
		Destroy( this.gameObject );
	}

	private void OnCollisionEnter( Collision other )
	{
		if ( this.dieOnCollision && this.state )
			this.Die();
	}
}
