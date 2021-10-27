using UnityEngine;

public class Unit : RootObject
{
	public float hp = 100f;
	public string fraction = "neutral";
	public Color color = new Color( 200, 200, 200 );

	[SerializeField]
	private float acceleration = 500f;

	[SerializeField]
	private float torque = 50f;

	private Vector3 _movementDirection = Vector3.zero;
	private Vector3 _rotatingDirection = Vector3.zero;
	private Vector3 _expectedMovementDirection = Vector3.zero;
	private Vector3 _expectedRotatingDirection = Vector3.zero;
	private Death _death;

	private new void Start ()
	{
		base.Start();
		this._death = this.gameObject.GetComponent<Death>();

		if ( this.renderer )
		{
			this.renderer.material.SetColor( "_Color", this.color );
			this.renderer.material.SetColor( "_EmissionColor", this.color );
		}
	}

	private void Update ()
	{
		this._movementDirection = Vector3.zero;
		this._rotatingDirection = Vector3.zero;

		if ( this._expectedMovementDirection != Vector3.zero )
		{
			this._movementDirection = this._expectedMovementDirection;
			this._expectedMovementDirection = Vector3.zero;
		}

		if ( this._expectedRotatingDirection != Vector3.zero )
		{
			this._rotatingDirection = this._expectedRotatingDirection;
			this._expectedRotatingDirection = Vector3.zero;
		}

		if ( this.hp <= 0 )
		{
			if ( this._death )
				this._death.Die();
			else
				Destroy( this.gameObject );
		}
	}

	public void Move ( Vector3 direction )
	{
		this._expectedMovementDirection += direction;

		if ( this._expectedMovementDirection.magnitude > Vector3.one.magnitude )
			this._expectedMovementDirection.Normalize();
	}

	public void Rotate ( Vector3 direction )
	{
		var angle = Vector3.SignedAngle( this.transform.forward, direction, this.transform.up );

		if ( angle < 0 )
			this._expectedRotatingDirection -= this.transform.up;
		else if ( angle > 0 )
			this._expectedRotatingDirection += this.transform.up;

		if ( this._expectedRotatingDirection.magnitude > Vector3.one.magnitude )
			this._expectedRotatingDirection.Normalize();
	}

	private void FixedUpdate()
	{
		this.rigidBody.AddForce( this._movementDirection * this.acceleration, ForceMode.Acceleration );
		this.rigidBody.AddTorque( this._rotatingDirection * this.torque, ForceMode.Acceleration );
	}
}
