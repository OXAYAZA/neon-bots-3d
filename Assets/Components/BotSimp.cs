using System;
using UnityEngine;

public class BotSimp : MonoBehaviour
{
	private ObjectData _data;
	private Unit _unit;
	private Gun _gun;
	private GameObject _target;
	private Vector3 _direction;
	private double _distance;

	private void Start()
	{
		this._data = this.GetComponent<ObjectData>();
		this._unit = this.GetComponent<Unit>();
		this._gun = this.GetComponent<Gun>();
	}

	private void Update()
	{
		this.Scan();

		if ( this._target )
		{
			this.Calculate();
			this.Rotate();
			this.Move();
			this.Attack();
		}
	}

	private void Scan ()
	{
		this._target = null;
		this._distance = Double.PositiveInfinity;

		var objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

		foreach ( var obj in objects )
		{
			var unit = obj.GetComponent<Unit>();
			var data = obj.GetComponent<ObjectData>();
			if ( obj != this.gameObject && unit && data && data.fraction != this._data.fraction )
			{
				var trs = obj.GetComponent<Transform>();
				var distance = Math.Sqrt( Math.Pow( trs.position.x - this.transform.position.x, 2 ) + Math.Pow( trs.position.z - this.transform.position.z, 2 ) );

				if ( distance < this._distance )
				{
					this._distance = distance;
					this._target = obj;
				}
			}
		}
	}

	private void Calculate ()
	{
		this._direction = new Vector3( this._target.transform.position.x - this.transform.position.x, 0, this._target.transform.position.z - this.transform.position.z );
	}

	private void Rotate ()
	{
		this._unit.Rotate( this._direction );
	}

	private void Move ()
	{
		if ( this._distance > 10 )
			this._unit.Move( this.transform.forward );
		else if ( this._distance < 8 )
			this._unit.Move( -this.transform.forward );
		else
			this._unit.Move( -this.transform.right );
	}

	private void Attack ()
	{
		if ( this._distance < 12 && this._gun )
			this._gun.Shot();
	}
}
