using UnityEngine;

public class RootObject : MonoBehaviour
{
	[HideInInspector]
	public GameObject body;

	[HideInInspector]
	public new Renderer renderer;

	[HideInInspector]
	public Rigidbody rigidBody;

	public void Start ()
	{
		this.body = this.transform.Find( "Body" ).gameObject;
		this.renderer = this.body.GetComponent<Renderer>();
		this.rigidBody = this.gameObject.GetComponent<Rigidbody>();
	}
}
