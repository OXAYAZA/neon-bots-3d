using UnityEngine;

public class Exit : MonoBehaviour
{
	[SerializeField]
	private string destinationScene;

	private new Renderer renderer;
	private Color initialColor;
	private bool activation = false;
	private float chargedValue = 5;
	private float chargeValue = 0;

	private void Start()
	{
		this.renderer = this.gameObject.GetComponent<Renderer>();
		this.initialColor = this.renderer.material.GetColor( "_EmissionColor" );
	}

	private void Update()
	{
		if ( this.activation )
			this.chargeValue = this.chargeValue < this.chargedValue ? this.chargeValue + Time.deltaTime : this.chargedValue;
		else
			this.chargeValue = this.chargeValue > 0 ? this.chargeValue - Time.deltaTime : 0;

		// if ( this.chargeValue >= this.chargedValue )
		// 	Root.Instance.GoToScene( this.destinationScene );

		this.renderer.material.SetColor( "_EmissionColor", Color.Lerp( this.initialColor, Color.white, this.chargeValue / this.chargedValue ) );
	}

	private void OnTriggerEnter ( Collider other )
	{
		// if ( other.gameObject.transform.parent.gameObject == Root.Instance.local.hero )
		// 	this.activation = true;
	}

	private void OnTriggerExit ( Collider other )
	{
		// if ( other.gameObject.transform.parent.gameObject == Root.Instance.local.hero )
		// 	this.activation = false;
	}
}
