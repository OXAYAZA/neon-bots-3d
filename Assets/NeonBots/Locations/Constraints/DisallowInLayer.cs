using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Locations.Constraints
{
    [CreateAssetMenu(fileName = "DisallowInLayer", menuName = "Neon Bots/Location/Constraints/DisallowInLayer", order = 2)]
    public class DisallowInLayer : InitialConstraint
    {
        public int layer;

        public override bool Check(List<VoxelTileData>[,,] data, Vector3Int position) =>
            this.layer < 0
                ? position.y != data.GetLength(1) + this.layer
                : position.y != this.layer;
    }
}
