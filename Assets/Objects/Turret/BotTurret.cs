using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BotTurret : MonoBehaviour
{
	private Unit _unit;
	private Gun _gun;
	private GameObject _target;
	private Vector3 _direction;
	private double _distance;
	private GameObject _visionSphere;

	[SerializeField]
	private float _visionRadius = 20;

	private void Start()
	{
		this._unit = this.GetComponent<Unit>();
		this._gun = this.GetComponent<Gun>();

		this._visionSphere = new GameObject( "VisionSphere" );

		this._visionSphere.transform.parent = this.gameObject.transform;
		this._visionSphere.transform.localPosition = Vector3.zero;
		this._visionSphere.transform.localRotation = Quaternion.identity;
		this._visionSphere.transform.localScale = new Vector3( this._visionRadius * 2, this._visionRadius * 2, this._visionRadius * 2 );

		this._visionSphere.AddComponent<MeshFilter>();
		var vsMeshFilter = this._visionSphere.GetComponent<MeshFilter>();
		var vsMeshSource = Root.Instance.primitives.Single( item => item.name == "Sphere" );
		vsMeshFilter.mesh = vsMeshSource.GetComponent<MeshFilter>().mesh;

		this._visionSphere.AddComponent<MeshRenderer>();
		var vsRenderer = this._visionSphere.GetComponent<Renderer>();
		vsRenderer.material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Objects/Turret/Vision.mat", typeof(Material));

		this._visionSphere.AddComponent<SphereCollider>();
		var vsCollider = this._visionSphere.GetComponent<Collider>();
		vsCollider.isTrigger = true;
	}

	private void Update()
	{
		if ( !this._target )
			this.Scan();

		else
			this.Tracking();

		if ( this._target )
		{
			this.Calculate();
			this.Rotate();
			this.Attack();
		}
	}

	private void Scan ()
	{
		var targetObjects = this.ScopeCheck();
		foreach ( var targetObject in targetObjects )
		{
			var target = targetObject.transform.parent.GetComponent<Unit>();
			if ( target && target.fraction != this._unit.fraction )
				this._target = targetObject;
		}
	}

	private void Tracking ()
	{
		var targetObjects = this.ScopeCheck();
		var inRange = targetObjects.Find( item => item == this._target );
		if ( !inRange || !this._target.GetComponent<Unit>() ) this._target = null;
	}

	private void Calculate ()
	{
		this._direction = new Vector3( this._target.transform.position.x - this.transform.position.x, 0, this._target.transform.position.z - this.transform.position.z );
		this._distance = Math.Sqrt( Math.Pow( this._target.transform.position.x - this.transform.position.x, 2 ) + Math.Pow( this._target.transform.position.z - this.transform.position.z, 2 ) );
	}

	private void Rotate ()
	{
		this._unit.Rotate( this._direction );
	}

	private void Attack ()
	{
		if ( this._gun && this._distance < this._visionRadius && Vector3.Angle( this.transform.forward, this._direction ) < 5f )
			this._gun.Shot();
	}

	private List<UnityEngine.GameObject> ScopeCheck ()
	{
		Collider[] hitColliders = Physics.OverlapSphere( this.transform.position, this._visionRadius, 1 << 3 );
		return hitColliders
			.Where( hitCollider => hitCollider.gameObject.transform.parent != this.transform )
			.Select( hitCollider => hitCollider.gameObject )
			.ToList();
	}
}
