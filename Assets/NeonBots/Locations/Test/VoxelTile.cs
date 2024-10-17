using System;
using UnityEngine;

namespace NeonBots.Locations
{
    public class VoxelTile : MonoBehaviour
    {
        // Front side of tile is looking in direction of Z+ axis.
        // So if we need data from front side, we need to look on it in Z- direction.
        public enum Side
        {
            Back, // Z+ (Vector3.forward)
            Right, // X- (Vector3.left)
            Front, // Z- (Vector3.back)
            Left // X+ (Vector3.right)
        }

        public enum Rotations
        {
            None,
            Two,
            Four
        }

        public int tileSize = 10;

        public float voxelSize = 0.1f;

        [Range(0f, 1f)]
        public float weight = 0.5f;

        public Rotations rotations;

        public MeshCollider meshCollider;

        public int[] sideBack;

        public int[] sideRight;

        public int[] sideFront;

        public int[] sideLeft;

        [ContextMenu("Generate Data")]
        public void GenerateData()
        {
            this.sideBack = new int[this.tileSize * this.tileSize];
            this.sideRight = new int[this.tileSize * this.tileSize];
            this.sideFront = new int[this.tileSize * this.tileSize];
            this.sideLeft = new int[this.tileSize * this.tileSize];

            for(var l = 0; l < this.tileSize; l++)
                for(var p = 0; p < this.tileSize; p++)
                    this.sideBack[l * this.tileSize + p] = this.GetVoxelSideColor(l, p, Side.Back);

            for(var l = 0; l < this.tileSize; l++)
                for(var p = 0; p < this.tileSize; p++)
                    this.sideRight[l * this.tileSize + p] = this.GetVoxelSideColor(l, p, Side.Right);

            for(var l = 0; l < this.tileSize; l++)
                for(var p = 0; p < this.tileSize; p++)
                    this.sideFront[l * this.tileSize + p] = this.GetVoxelSideColor(l, p, Side.Front);

            for(var l = 0; l < this.tileSize; l++)
                for(var p = 0; p < this.tileSize; p++)
                    this.sideLeft[l * this.tileSize + p] = this.GetVoxelSideColor(l, p, Side.Left);
        }

        // If we look on any side, we count voxel position from left to right.
        // Layer position from bottom to top.
        // If we need to compare opposite tiles, we need to mirror comparable tile side data vertically.
        private int GetVoxelSideColor(int layer, int position, Side side)
        {
            var voxelHalf = this.voxelSize * 0.5f;
            var bounds = this.meshCollider.bounds;

            var rayDirection = side switch
            {
                Side.Back => Vector3.forward,
                Side.Right => Vector3.left,
                Side.Front => Vector3.back,
                Side.Left => Vector3.right,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            var rayStart = side switch
            {
                Side.Back => bounds.min + new Vector3(
                    voxelHalf + position * this.voxelSize,
                    voxelHalf + layer * this.voxelSize,
                    -voxelHalf
                ),
                Side.Right => bounds.min + new Vector3(bounds.size.x, 0f, 0f) + new Vector3(
                    voxelHalf,
                    voxelHalf + layer * this.voxelSize,
                    voxelHalf + position * this.voxelSize
                ),
                Side.Front => bounds.min + new Vector3(bounds.size.x, 0f, bounds.size.z) + new Vector3(
                    -voxelHalf - position * this.voxelSize,
                    voxelHalf + layer * this.voxelSize,
                    voxelHalf
                ),
                Side.Left => bounds.min + new Vector3(0f, 0f, bounds.size.z) + new Vector3(
                    -voxelHalf,
                    voxelHalf + layer * this.voxelSize,
                    -voxelHalf - position * this.voxelSize
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            var ray = new Ray(rayStart, rayDirection);

            if(Physics.Raycast(ray, out var hit, this.voxelSize)) return (int)(hit.textureCoord.x * 256);
            return 0;
        }

        public void Rotate90()
        {
            this.transform.Rotate(0f, 90f, 0f);

            var sideBack = new int[this.sideBack.Length];
            this.sideBack.CopyTo(sideBack, 0);
            var sideRight = new int[this.sideRight.Length];
            this.sideRight.CopyTo(sideRight, 0);
            var sideFront = new int[this.sideFront.Length];
            this.sideFront.CopyTo(sideFront, 0);
            var sideLeft = new int[this.sideLeft.Length];
            this.sideLeft.CopyTo(sideLeft, 0);

            this.sideBack = sideRight;
            this.sideRight = sideFront;
            this.sideFront = sideLeft;
            this.sideLeft = sideBack;
        }
    }
}
