using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Locations.Constraints
{
    [CreateAssetMenu(fileName = "AllowInColumn", menuName = "Neon Bots/Location/Constraints/AllowInColumn", order = 1)]
    public class AllowInColumn : ActiveConstraint
    {
        [SerializeField]
        protected List<VoxelTileData> allowed;

        public override bool Apply(VoxelTileData sample, Vector3Int current, Vector3Int start) =>
            current.x != start.x || current.z != start.z || (this.allowed != default &&
                this.allowed.Exists(allowed => sample.name == allowed.name));
    }
}
