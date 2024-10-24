using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Locations
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "Neon Bots/Location/Tile Set", order = 1)]
    public class TileSet : ScriptableObject
    {
        public float tileSize = 1f;

        public float tileScale = 1f;

        public List<VoxelTileData> samples;
    }
}
