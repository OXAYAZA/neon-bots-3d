using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
	private Unit hero;
	private RectTransform bar;
	private Text text;
	private float initialHP;
	private float initialWidth;

	void Start()
	{
		this.hero = Root.Instance.hero.GetComponent<Unit>();
		this.bar = this.transform.Find( "Percentage" ).gameObject.GetComponent<RectTransform>();
		this.text = this.transform.Find( "Text" ).gameObject.GetComponent<Text>();
		this.initialWidth = this.bar.sizeDelta.x;
		this.initialHP = this.hero.hp;
	}

	void Update()
	{
		this.bar.sizeDelta = new Vector2( this.initialWidth * ( this.hero.hp / this.initialHP ), this.bar.sizeDelta.y );
		this.text.text = "HP: " + this.hero.hp;
	}
}
