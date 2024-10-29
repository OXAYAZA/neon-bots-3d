using UnityEngine;

namespace NeonBots.Locations.Constraints
{
    public abstract class ActiveConstraint : Constraint
    {
        public abstract bool Apply(VoxelTileData sample, Vector3Int start, Vector3Int current);
    }
}
