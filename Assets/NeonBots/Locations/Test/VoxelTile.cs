using System;
using System.Reflection;
using UnityEditor;
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

        public MeshCollider meshCollider;

        [SerializeField]
        private float voxelSize = 0.1f;

        [SerializeField]
        private int tileDimension = 10;

        public VoxelTileData GenerateData()
        {
            var data = ScriptableObject.CreateInstance<VoxelTileData>();
            data.back = new(this.tileDimension);
            data.right = new(this.tileDimension);
            data.front = new(this.tileDimension);
            data.left = new(this.tileDimension);

            for(var l = 0; l < this.tileDimension; l++)
                for(var p = 0; p < this.tileDimension; p++)
                    data.back.data[l * this.tileDimension + p] = this.GetVoxelSideColor(l, p, Side.Back);

            for(var l = 0; l < this.tileDimension; l++)
                for(var p = 0; p < this.tileDimension; p++)
                    data.right.data[l * this.tileDimension + p] = this.GetVoxelSideColor(l, p, Side.Right);

            for(var l = 0; l < this.tileDimension; l++)
                for(var p = 0; p < this.tileDimension; p++)
                    data.front.data[l * this.tileDimension + p] = this.GetVoxelSideColor(l, p, Side.Front);

            for(var l = 0; l < this.tileDimension; l++)
                for(var p = 0; p < this.tileDimension; p++)
                    data.left.data[l * this.tileDimension + p] = this.GetVoxelSideColor(l, p, Side.Left);

            return data;
        }

        [ContextMenu("Generate VoxelTileData")]
        public void SaveData()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            var obj = getActiveFolderPath!.Invoke(null, Array.Empty<object>());
            var pathToCurrentFolder = obj.ToString();

            var data = this.GenerateData();
            AssetDatabase.CreateAsset(data, $"{pathToCurrentFolder}/{this.name}Data.asset");
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
    }
}
