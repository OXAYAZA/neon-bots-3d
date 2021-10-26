using System;
using UnityEngine;

public class BotTurret : MonoBehaviour
{
	private Unit _unit;
	private Gun _gun;
	private GameObject _target;
	private Vector3 _direction;
	private double _distance;
	private GameObject _visionSphere;

	[SerializeField]
	private float _visionRadius = 10;

	private void Start()
	{
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
			if ( unit && obj != this.gameObject && unit.fraction != this._unit.fraction )
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

	private void Attack ()
	{
		if ( this._distance < 20 && this._gun )
			this._gun.Shot();
	}
}
