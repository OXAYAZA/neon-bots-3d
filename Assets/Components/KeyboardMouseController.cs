using UnityEngine;

public class KeyboardMouseController : MonoBehaviour
{
	private new GameObject camera;
	private CameraController cameraController;
	private Unit hero;
	private Gun gun;
	private Vector3 _cursorPos = Vector3.zero;

	public void Start ()
	{
		this.Reconf();
		Root.Instance.reconfEvent.AddListener( this.Reconf );
	}

	public void Reconf ()
	{
		this.camera = Root.Instance.camera;

		if ( Root.Instance.local.hero )
		{
			this.hero = Root.Instance.local.hero.GetComponent<Unit>();
			this.gun = Root.Instance.local.hero.GetComponent<Gun>();
		}

		if ( this.camera )
		{
			this.cameraController = this.camera.GetComponent<CameraController>();
			this._cursorPos = this.cameraController.cursor;
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
			if ( this.camera )
			{
				this._cursorPos = this.cameraController.cursor;

				if ( this._cursorPos != this.camera.transform.position )
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
