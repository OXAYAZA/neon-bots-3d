using UnityEngine;

namespace Components
{
	public class TouchRotation : TouchController
	{
		private Unit hero;
		private Gun gun;

		private new void Start ()
		{
			base.Start();
			this.hero = Root.Instance.hero.GetComponent<Unit>();
			this.gun = Root.Instance.hero.GetComponent<Gun>();
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
}
