using UnityEngine;

public class KeyboardMouseController : MonoBehaviour
{
	[SerializeField]
	private GameObject cameraObj;

	private Unit hero;
	private Gun gun;
	private Vector3 _cursorPos = Vector3.zero;

	private void Start ()
	{
		this.hero = Root.Instance.hero.GetComponent<Unit>();
		this.gun = Root.Instance.hero.GetComponent<Gun>();

		if ( this.cameraObj )
		{
			CameraController controller = this.cameraObj.GetComponent<CameraController>();
			this._cursorPos = controller.cursor;
		}
	}

	private void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Debug.Log( "Escape" );
			Root.Instance.PlayPauseGame();
		}

		if ( this.hero )
		{
			if ( this.cameraObj )
			{
				CameraController controller = this.cameraObj.GetComponent<CameraController>();
				this._cursorPos = controller.cursor;

				if ( this._cursorPos != this.cameraObj.transform.position )
				{
					var direction = new Vector3( this._cursorPos.x - this.hero.transform.position.x, 0, this._cursorPos.z - this.hero.transform.position.z );
					this.hero.Rotate( direction );
				}
			}

			if ( Input.GetKey( KeyCode.A ) )
				this.hero.Move( Vector3.left );

			if ( Input.GetKey( KeyCode.D ) )
				this.hero.Move( Vector3.right );

			if ( Input.GetKey( KeyCode.S ) )
				this.hero.Move( Vector3.back );

			if ( Input.GetKey( KeyCode.W ) )
				this.hero.Move( Vector3.forward );

			if ( Input.GetMouseButton( 0 ) && this.gun )
				this.gun.Shot();
		}
	}
}
