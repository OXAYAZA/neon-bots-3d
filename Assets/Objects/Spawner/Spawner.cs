using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public string fraction = "none";
	public Color color = new Color( 200, 200, 200 );

	[SerializeField]
	private GameObject obj;

	[SerializeField]
	private float _spawnPeriod = 5f;

	private float _spawnTimer;
	private Renderer _renderer;
	private List<GameObject> _triggers = new List<GameObject>();

	private void Start()
	{
		this._spawnTimer = this._spawnPeriod;
		this._renderer = this.GetComponent<Renderer>();
		
		this._renderer.material.SetColor( "_Color", this.color );
		this._renderer.material.SetColor( "_EmissionColor", this.color );
	}

	private void SpawnObject()
	{
		var initialTransform = this.transform;
		var tmp = Instantiate( this.obj, initialTransform.position, initialTransform.rotation );
		var tmpUnit = tmp.GetComponent<Unit>();

		if ( tmpUnit )
		{
			tmpUnit.fraction = this.fraction;
			tmpUnit.color = this.color;
		}
	}

	private void Update ()
	{
		if ( this._spawnTimer <= 0 )
		{
			if ( this._triggers.Count <= 0 )
			{
				this.SpawnObject();
			}

			this._spawnTimer = this._spawnPeriod;
		}

		else
		{
			this._spawnTimer -= Time.deltaTime;
		}

		if ( this._triggers.Count != 0 )
		{
			var busyColor = new Color( 1, 1, 1, 0.5f );
			this._renderer.material.SetColor( "_Color", busyColor );
			this._renderer.material.SetColor( "_EmissionColor", busyColor );
		}

		else
		{
			this._renderer.material.SetColor( "_Color", this.color );
			this._renderer.material.SetColor( "_EmissionColor", this.color );
		}
	}

	private void OnTriggerEnter( Collider other )
	{
		var component = other.gameObject.GetComponent<Unit>();

		if ( component )
			this._triggers.Add( other.gameObject );
	}

	private void OnTriggerExit( Collider other )
	{
		this._triggers.Remove( other.gameObject );
	}
}
