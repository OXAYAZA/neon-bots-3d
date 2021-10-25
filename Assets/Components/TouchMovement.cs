using UnityEngine;

namespace Components
{
	public class TouchMovement : TouchController
	{
		private Unit hero;

		private new void Start ()
		{
			base.Start();
			this.hero = Root.Instance.hero.GetComponent<Unit>();
		}

		private new void Update ()
		{
			base.Update();
			this.hero.Move( this.controlVector );
		}
	}
}
