using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	[HideInInspector]
	public Vector3 controlVector = new Vector3( 0, 0, 0);

	[HideInInspector]
	public bool state = false;

	private Text text;
	private RectTransform stickTransform;
	private RectTransform rectTransform;
	private Vector2 panelPosition;
	private Vector2 centerPosition;
	private Vector2 cursorPosition = new Vector2( 0, 0 );
	private int _lastScreenWidth;
	private int _lastScreenHeight;

	public void Start ()
	{
		this.text = this.transform.Find( "Text" ).gameObject.GetComponent<Text>();
		this.stickTransform = this.transform.Find( "Stick" ).gameObject.GetComponent<RectTransform>();
		this.rectTransform = this.gameObject.GetComponent<RectTransform>();
		this.Resize();
	}

	public void Update ()
	{
		if ( this._lastScreenWidth != Screen.width || this._lastScreenHeight != Screen.height )
			this.Resize();

		this._lastScreenWidth = Screen.width;
		this._lastScreenHeight = Screen.height;

		this.stickTransform.anchoredPosition = this.cursorPosition;
		this.controlVector = new Vector3( this.cursorPosition.x / this.centerPosition.x, 0, this.cursorPosition.y / this.centerPosition.y );
		this.text.text = this.controlVector.ToString();
	}

	public void OnPointerDown ( PointerEventData eventData )
	{
		this.state = true;
		this.cursorPosition = eventData.position - this.panelPosition - this.centerPosition;
	}

	public void OnPointerUp ( PointerEventData eventData )
	{
		this.state = false;
		this.cursorPosition = new Vector2( 0, 0 );
	}

	public void OnDrag ( PointerEventData eventData )
	{
		this.cursorPosition = eventData.position - this.panelPosition - this.centerPosition;
	}

	private void Resize ()
	{
		this.panelPosition = new Vector2( Screen.width, Screen.height ) * this.rectTransform.anchorMax + this.rectTransform.anchoredPosition;
		this.centerPosition = this.rectTransform.sizeDelta/2;
	}
}
