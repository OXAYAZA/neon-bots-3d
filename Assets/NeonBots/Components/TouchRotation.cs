public class TouchRotation : TouchController
{
	private Unit hero;
	private Gun gun;

	private new void Start ()
	{
		base.Start();
		this.Reconf();
		Root.Instance.reconfEvent.AddListener( this.Reconf );
	}
	
	public void Reconf ()
	{
		if ( Root.Instance.local.hero )
		{
			this.hero = Root.Instance.local.hero.GetComponent<Unit>();
			this.gun = Root.Instance.local.hero.GetComponent<Gun>();
		}
	}

	private new void Update ()
	{
		base.Update();

		if ( this.hero )
			this.hero.Rotate( this.controlVector );

		if ( this.state && this.gun )
			this.gun.Shot();
	}
}
