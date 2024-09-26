using UnityEngine;

namespace NeonBots.Components
{
    public class TouchController : MonoBehaviour
    {
        private Unit unit;

        private Gun gun;

        private void Init()
        {
            this.unit = this.GetComponent<Unit>();
            this.gun = this.GetComponent<Gun>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape");
                //Root.Instance.PlayPauseGame();
            }

            if(this.unit)
            {
                // this.unit.Move( this.moveVector );
                // this.unit.Rotate( this.rotateVector );
            }
        }
    }
}