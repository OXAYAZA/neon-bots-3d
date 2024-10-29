using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NeonBots.Locations
{
    public abstract class LocationGenerator : MonoBehaviour
    {
        public abstract UniTask Generate();

        public abstract Vector3 GetStartPosition();
    }
}
