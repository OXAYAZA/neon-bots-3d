using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonBots.Locations
{
    [CreateAssetMenu(fileName = "VoxelTileData", menuName = "Neon Bots/Location/Voxel Tile Data", order = 1)]
    public class VoxelTileData : ScriptableObject
    {
        public enum Constraint
        {
            OneInColumn,
            RestrictedAtBottom,
            RestrictedAtTop
        }

        public VoxelTile tile;

        [Range(0f, 1f)]
        public float weight = 0.5f;

        public VoxelTileRotations rotations;

        public List<Constraint> constraints;

        public VoxelTileSide back;

        public VoxelTileSide right;

        public VoxelTileSide front;

        public VoxelTileSide left;

        public VoxelTileSide top;

        public VoxelTileSide bottom;

        public Vector3 rotation = Vector3.zero;

        public bool CompareSide(VoxelTile.Side side, VoxelTileData target)
        {
            return side switch
            {
                VoxelTile.Side.Back => this.back.data.SequenceEqual(target.front.Mirrored()),
                VoxelTile.Side.Right => this.right.data.SequenceEqual(target.left.Mirrored()),
                VoxelTile.Side.Front => this.front.data.SequenceEqual(target.back.Mirrored()),
                VoxelTile.Side.Left => this.left.data.SequenceEqual(target.right.Mirrored()),
                VoxelTile.Side.Top => this.top.data.SequenceEqual(target.bottom.data),
                VoxelTile.Side.Bottom => this.bottom.data.SequenceEqual(target.top.data),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }

        public bool CheckConstraints()
        {
            foreach(var constraint in this.constraints)
            {
                switch(constraint)
                {
                    case Constraint.OneInColumn:
                        break;
                    case Constraint.RestrictedAtBottom:
                        break;
                    case Constraint.RestrictedAtTop:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

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
            var top = new VoxelTileSide(this.top.size);
            this.top.CopyTo(top);
            var bottom = new VoxelTileSide(this.bottom.size);
            this.bottom.CopyTo(bottom);

            this.back = right;
            this.right = front;
            this.front = left;
            this.left = back;

            {
                for(var l = 0; l < this.top.size; l++)
                    for(var p = 0; p < this.top.size; p++)
                        this.top.data[p * this.top.size + this.top.size - l - 1] = top.data[l * this.top.size + p];
            }

            {
                for(var l = 0; l < this.top.size; l++)
                    for(var p = 0; p < this.top.size; p++)
                        this.bottom.data[p * this.top.size + this.top.size - l - 1] = bottom.data[l * this.top.size + p];
            }
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
            Array.Reverse(this.top.data);
            Array.Reverse(this.bottom.data);
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
            var top = new VoxelTileSide(this.top.size);
            this.top.CopyTo(top);
            var bottom = new VoxelTileSide(this.bottom.size);
            this.bottom.CopyTo(bottom);

            this.back = left;
            this.right = back;
            this.front = right;
            this.left = front;

            {
                for(var l = 0; l < this.top.size; l++)
                    for(var p = 0; p < this.top.size; p++)
                        this.top.data[(this.top.size - p - 1) * this.top.size + l] = top.data[l * this.top.size + p];
            }

            {
                for(var l = 0; l < this.top.size; l++)
                    for(var p = 0; p < this.top.size; p++)
                        this.bottom.data[(this.top.size - p - 1) * this.top.size + l] = bottom.data[l * this.top.size + p];
            }
        }
    }
}
