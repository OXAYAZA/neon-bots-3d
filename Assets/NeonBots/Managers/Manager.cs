using UnityEngine;

namespace NeonBots.Managers
{
    public abstract class Manager : MonoBehaviour
    {
        public bool IsReady { get; protected set; }
    }
}
