using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Locations.Constraints
{
    public abstract class InitialConstraint : Constraint
    {
        public abstract bool Check(List<VoxelTileData>[,,] data, Vector3Int position);
    }
}
