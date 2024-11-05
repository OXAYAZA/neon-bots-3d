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
            Left, // X+ (Vector3.right)
            Top, // Y- (Vector3.down)
            Bottom // Y+ (Vector3.up)
        }

        [SerializeField]
        private float tileSize = 1f;

        [SerializeField]
        private int tileDimension = 10;

        private float voxelSize;

        public VoxelTileData GenerateData()
        {
            this.voxelSize = this.tileSize / this.tileDimension;

            var data = ScriptableObject.CreateInstance<VoxelTileData>();
            data.back = new(this.tileDimension);
            data.right = new(this.tileDimension);
            data.front = new(this.tileDimension);
            data.left = new(this.tileDimension);
            data.top = new(this.tileDimension);
            data.bottom = new(this.tileDimension);

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

            for(var l = 0; l < this.tileDimension; l++)
                for(var p = 0; p < this.tileDimension; p++)
                    data.top.data[l * this.tileDimension + p] = this.GetVoxelSideColor(l, p, Side.Top);

            for(var l = 0; l < this.tileDimension; l++)
                for(var p = 0; p < this.tileDimension; p++)
                    data.bottom.data[l * this.tileDimension + p] = this.GetVoxelSideColor(l, p, Side.Bottom);

            return data;
        }

#if UNITY_EDITOR
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
#endif

        // If we look on any side, we count voxel position from left to right.
        // Layer position from bottom to top.
        // If we need to compare opposite tiles, we need to mirror comparable tile side data vertically.
        private int GetVoxelSideColor(int layer, int position, Side side)
        {
            var tileHalf = this.tileSize * 0.5f;
            var voxelHalf = this.voxelSize * 0.5f;
            var start = this.transform.position - Vector3.one * tileHalf;

            var rayDirection = side switch
            {
                Side.Back => Vector3.forward,
                Side.Right => Vector3.left,
                Side.Front => Vector3.back,
                Side.Left => Vector3.right,
                Side.Top => Vector3.down,
                Side.Bottom => Vector3.up,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            var rayStart = side switch
            {
                Side.Back => start + new Vector3(
                    voxelHalf + position * this.voxelSize,
                    voxelHalf + layer * this.voxelSize,
                    -voxelHalf
                ),
                Side.Right => start + new Vector3(this.tileSize, 0f, 0f) + new Vector3(
                    voxelHalf,
                    voxelHalf + layer * this.voxelSize,
                    voxelHalf + position * this.voxelSize
                ),
                Side.Front => start + new Vector3(this.tileSize, 0f, this.tileSize) + new Vector3(
                    -voxelHalf - position * this.voxelSize,
                    voxelHalf + layer * this.voxelSize,
                    voxelHalf
                ),
                Side.Left => start + new Vector3(0f, 0f, this.tileSize) + new Vector3(
                    -voxelHalf,
                    voxelHalf + layer * this.voxelSize,
                    -voxelHalf - position * this.voxelSize
                ),
                Side.Top => start + new Vector3(0f, this.tileSize, 0f) + new Vector3(
                    voxelHalf + position * this.voxelSize,
                    voxelHalf,
                    voxelHalf + layer * this.voxelSize
                ),
                Side.Bottom => start + new Vector3(0f, 0f, 0f) + new Vector3(
                    voxelHalf + position * this.voxelSize,
                    -voxelHalf,
                    voxelHalf + layer * this.voxelSize
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            var ray = new Ray(rayStart, rayDirection);

            if(Physics.Raycast(ray, out var hit, this.voxelSize)) return (int)(hit.textureCoord.x * 256);

            Debug.DrawLine(rayStart, rayStart + rayDirection * this.voxelSize, Color.red, 3f);

            return 0;
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(this.transform.position, this.transform.localScale * this.tileSize);

            Gizmos.color = initialColor;
        }
    }
}
