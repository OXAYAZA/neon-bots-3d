using UnityEngine;

public class CameraController : MonoBehaviour
{
	[HideInInspector]
	public Vector3 cursor;

	[SerializeField]
	private GameObject target;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private Vector3 defaultPosition;

	private Camera _cam;

	private void Start()
	{
		this._cam = this.GetComponent<Camera>();
		Root.Instance.reconfEvent.AddListener( this.Reconf );

		if ( !this.target )
			this.transform.position = this.defaultPosition;
	}

	public void Reconf ()
	{
		if ( Root.Instance.local.hero )
			this.target = Root.Instance.local.hero;
		else
			this.transform.position = this.defaultPosition;
	}

	private void Update()
	{
		if ( this.target )
			this.transform.position = this.target.transform.position + this.offset;

		RaycastHit hit;
		var ray = this._cam.ScreenPointToRay( Input.mousePosition );
		var tmp = Physics.Raycast( ray.origin, ray.direction, out hit, 200, 1 << 7 );
		this.cursor = tmp ? hit.point : this.transform.position;
	}

	private void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( this.cursor, 1f );
	}
}
