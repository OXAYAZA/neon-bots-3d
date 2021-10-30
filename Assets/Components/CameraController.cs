using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector3 cursor;

	[SerializeField]
	private GameObject target;

	private Vector3 _offset;
	private Camera _cam;
	private Transform _point;

	private void Start()
	{
		if ( this.target )
			this._offset = this.transform.position - this.target.transform.position;

		this._cam = this.GetComponent<Camera>();
		this._point = this.transform.Find( "Point" );
	}

	private void Update()
	{
		if ( this.target )
			this.transform.position = this.target.transform.position + this._offset;

		RaycastHit hit;
		var ray = this._cam.ScreenPointToRay( Input.mousePosition );
		var tmp = Physics.Raycast( ray.origin, ray.direction, out hit, 200, 1 << 7 );
		this.cursor = tmp ? hit.point : this.transform.position;
		this._point.position = this.cursor;
	}
}
