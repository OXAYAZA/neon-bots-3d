using UnityEngine;

namespace NeonBots.Components
{
    public class Bot : Controller
    {
        public float scanRange = 30f;

        public int scanNumber = 20;

        public float shotRange = 30f;

        [SerializeField]
        protected LayerMask layerMask;

        protected Unit unit;
    }
}
