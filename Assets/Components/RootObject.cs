using UnityEngine;

public class RootObject : MonoBehaviour
{
	[HideInInspector]
	public GameObject body;

	[HideInInspector]
	public new Renderer renderer;

	[HideInInspector]
	public new Rigidbody rigidbody;

	private void Start ()
	{
		this.body = this.transform.Find( "Body" ).gameObject;
		this.renderer = this.body.GetComponent<Renderer>();
		this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
	}
}
