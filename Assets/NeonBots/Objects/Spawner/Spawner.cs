using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	private GameObject obj;

	[SerializeField]
	private float _spawnPeriod = 5f;

	private float _spawnTimer;
	private ObjectData data;
	private new Renderer renderer;
	private List<GameObject> _triggers = new List<GameObject>();

	private void Start()
	{
		this.data = this.gameObject.GetComponent<ObjectData>();
		this.renderer = this.transform.Find("Body").gameObject.GetComponent<Renderer>();
		this._spawnTimer = this._spawnPeriod;
	}

	private void SpawnObject()
	{
		var initialTransform = this.transform;
		var tmp = Instantiate( this.obj, initialTransform.position, initialTransform.rotation );
		var tmpData = tmp.GetComponent<ObjectData>();

		if ( tmpData )
		{
			tmpData.fraction = this.data.fraction;
			tmpData.color = this.data.color;
		}
	}

	private void Update ()
	{
		if ( this._spawnTimer <= 0 )
		{
			if ( this._triggers.Count <= 0 )
				this.SpawnObject();

			this._spawnTimer = this._spawnPeriod;
		}

		else
		{
			this._spawnTimer -= Time.deltaTime;
		}

		if ( this._triggers.Count != 0 )
		{
			var busyColor = new Color( 1, 1, 1, 0.5f );
			this.renderer.material.SetColor( "_Color", busyColor );
			this.renderer.material.SetColor( "_EmissionColor", busyColor );
		}

		else
		{
			this.renderer.material.SetColor( "_Color", this.data.color );
			this.renderer.material.SetColor( "_EmissionColor", this.data.color );
		}
	}

	private void OnTriggerEnter( Collider other )
	{
		if ( other.gameObject.layer == 3 )
			this._triggers.Add( other.gameObject );
	}

	private void OnTriggerExit( Collider other )
	{
		this._triggers.Remove( other.gameObject );
	}
}
