using UnityEngine;

namespace NeonBots.Locations
{
    [CreateAssetMenu(fileName = "VoxelTileData", menuName = "Neon Bots/Location/Voxel Tile Data", order = 1)]
    public class VoxelTileData : ScriptableObject
    {
        public VoxelTile tile;

        [Range(0f, 1f)]
        public float weight = 0.5f;

        public VoxelTileRotations rotations;

        public VoxelTileSide back;

        public VoxelTileSide right;

        public VoxelTileSide front;

        public VoxelTileSide left;

        public Vector3 rotation = Vector3.zero;

        public void Rotate90()
        {
            this.rotation.y = 90f;

            var back = new VoxelTileSide(this.back.size);
            this.back.CopyTo(back);
            var right = new VoxelTileSide(this.right.size);
            this.right.CopyTo(right);
            var front = new VoxelTileSide(this.front.size);
            this.front.CopyTo(front);
            var left = new VoxelTileSide(this.left.size);
            this.left.CopyTo(left);

            this.back = right;
            this.right = front;
            this.front = left;
            this.left = back;
        }

        public void Rotate180()
        {
            this.rotation.y = 180f;

            var back = new VoxelTileSide(this.back.size);
            this.back.CopyTo(back);
            var right = new VoxelTileSide(this.right.size);
            this.right.CopyTo(right);
            var front = new VoxelTileSide(this.front.size);
            this.front.CopyTo(front);
            var left = new VoxelTileSide(this.left.size);
            this.left.CopyTo(left);

            this.back = front;
            this.right = left;
            this.front = back;
            this.left = right;
        }

        public void Rotate270()
        {
            this.rotation.y = 270f;

            var back = new VoxelTileSide(this.back.size);
            this.back.CopyTo(back);
            var right = new VoxelTileSide(this.right.size);
            this.right.CopyTo(right);
            var front = new VoxelTileSide(this.front.size);
            this.front.CopyTo(front);
            var left = new VoxelTileSide(this.left.size);
            this.left.CopyTo(left);

            this.back = left;
            this.right = back;
            this.front = right;
            this.left = front;
        }
    }
}
