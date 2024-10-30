using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Locations.Constraints
{
    [CreateAssetMenu(fileName = "AllowInRange", menuName = "Neon Bots/Location/Constraints/AllowInRange", order = 3)]
    public class AllowInRange : InitialConstraint
    {
        public Vector3Int min = Vector3Int.zero;

        public Vector3Int max = new(-1, -1, -1);

        public override bool Check(List<VoxelTileData>[,,] data, Vector3Int position)
        {
            var min = this.Convert(data, this.min);
            var max = this.Convert(data, this.max);

            if(!this.Validate(min, max)) return true;

            return
                position.x >= min.x && position.y >= min.y && position.z >= min.z &&
                position.x <= max.x && position.y <= max.y && position.z <= max.z;
        }

        private Vector3Int Convert(List<VoxelTileData>[,,] data, Vector3Int value)
        {
            var val = value;

            if(val.x < 0) val.x = data.GetLength(0) + val.x;
            if(val.y < 0) val.y = data.GetLength(1) + val.y;
            if(val.z < 0) val.z = data.GetLength(2) + val.z;

            return val;
        }

        private bool Validate(Vector3Int min, Vector3Int max)
        {
            if(min.x > max.x)
            {
                Debug.LogWarning($"{this.name} Constraint Error: Min X cannot be greater than Max X. Min: {min}; Max: {max}.");
                return false;
            }

            if(min.y > max.y)
            {
                Debug.LogWarning($"{this.name} Constraint Error: Min Y cannot be greater than Max Y. Min: {min}; Max: {max}.");
                return false;
            }

            if(min.z > max.z)
            {
                Debug.LogWarning($"{this.name} Constraint Error: Min Z cannot be greater than Max Z. Min: {min}; Max: {max}.");
                return false;
            }

            return true;
        }
    }
}
