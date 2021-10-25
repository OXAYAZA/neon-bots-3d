using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleControl : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private InputType inputType;
	private Toggle toggle;

	void Start ()
	{
		this.toggle = this.gameObject.GetComponent<Toggle>();

		if ( this.inputType == Root.Instance.controlType )
			this.toggle.isOn = true;
	}

	public void OnPointerClick( PointerEventData eventData )
	{
		if ( Root.Instance.controlType != this.inputType )
			Root.Instance.SwitchControl( this.inputType );
	}
}
