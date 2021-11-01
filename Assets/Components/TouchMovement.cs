public class TouchMovement : TouchController
{
	private Unit hero;

	private new void Start ()
	{
		base.Start();
		this.Reconf();
		Root.Instance.reconfEvent.AddListener( this.Reconf );
	}

	public void Reconf ()
	{
		if ( Root.Instance.local.hero )
			this.hero = Root.Instance.local.hero.GetComponent<Unit>();
	}

	private new void Update ()
	{
		base.Update();
		this.hero.Move( this.controlVector );
	}
}
