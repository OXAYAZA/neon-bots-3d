using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeonBots.Locations
{
    public class LocationGeneratorSimple : MonoBehaviour
    {
        public Vector2Int locationSize = new(10, 10);

        public float tileSize = 1f;

        public List<VoxelTileData> samples;

        public List<VoxelTile> tiles;

        private List<VoxelTileData> workSamples;

        private VoxelTileData[,] data;

        private bool isReady;

        private void Start()
        {
            this.ProcessSamples();
            this.isReady = true;
        }

        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            if(!Application.isPlaying || !this.isReady) return;

            this.Generate();

            if(this.tiles.Count > 0)
            {
                foreach(var tile in this.tiles) Destroy(tile.gameObject);
                this.tiles.Clear();
            }

            var center = this.transform.position;
            var size = new Vector3(this.locationSize.x * this.tileSize, 0f, this.locationSize.y * this.tileSize);
            var startPosition = center - size * 0.5f - new Vector3(this.tileSize, 0f, this.tileSize) * 0.5f;

            for(var y = 1; y < this.data.GetLength(1) - 1; y++)
            {
                for(var x = 1; x < this.data.GetLength(0) - 1; x++)
                {
                    var data = this.data[x, y];

                    if(data == null) continue;

                    var position = startPosition + new Vector3(x, 0f, y) * this.tileSize;
                    var tile = Instantiate(data.tile, position, Quaternion.Euler(data.rotation));
                    this.tiles.Add(tile);
                }
            }
        }

        private void ProcessSamples()
        {
            this.workSamples ??= new();

            foreach(var sample in this.samples)
            {
                switch(sample.rotations)
                {
                    case VoxelTileRotations.None:
                    {
                        var clone = Instantiate(sample);
                        this.workSamples.Add(clone);
                        break;
                    }
                    case VoxelTileRotations.Two:
                    {
                        var weight = sample.weight * 0.5f;
                        var clone = Instantiate(sample);
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate90();
                        clone.weight = weight;
                        this.workSamples.Add(clone);
                        break;
                    }
                    case VoxelTileRotations.Four:
                    {
                        var weight = sample.weight * 0.25f;
                        var clone = Instantiate(sample);
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate90();
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate180();
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate270();
                        clone.weight = weight;
                        this.workSamples.Add(clone);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Generate()
        {
            // Here, empty tiles are added as fields.
            this.data = new VoxelTileData[this.locationSize.x + 2, this.locationSize.y + 2];

            for(var y = 1; y < this.data.GetLength(1) - 1; y++)
            {
                for(var x = 1; x < this.data.GetLength(0) - 1; x++)
                {
                    var filteredSamples = this.workSamples.Where(tile =>
                        this.CanAppendTile(this.data[x, y - 1], tile, VoxelTile.Side.Front) &&
                        this.CanAppendTile(this.data[x + 1, y], tile, VoxelTile.Side.Left) &&
                        this.CanAppendTile(this.data[x, y + 1], tile, VoxelTile.Side.Back) &&
                        this.CanAppendTile(this.data[x - 1, y], tile, VoxelTile.Side.Right)).ToList();

                    if(filteredSamples.Count == 0) continue;

                    this.data[x, y] = this.RandomTile(filteredSamples);
                }
            }
        }

        private bool CanAppendTile(VoxelTileData target, VoxelTileData comparable, VoxelTile.Side side) =>
            target == default || target.CompareSide(side, comparable);

        private VoxelTileData RandomTile(List<VoxelTileData> tiles)
        {
            var weights = tiles.Select(item => item.weight).ToList();
            var value = Random.Range(0f, weights.Sum());
            var sum = 0f;

            for(var i = 0; i < weights.Count; i++)
            {
                sum += weights[i];
                if(value < sum) return tiles[i];
            }

            return tiles[^1];
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;

            Gizmos.color = Color.cyan;
            var center = this.transform.position;
            var size = new Vector3(this.locationSize.x * this.tileSize, 0f, this.locationSize.y * this.tileSize);
            var halfSize = size * 0.5f;
            var c1 = center + new Vector3(-halfSize.x, 0f, -halfSize.z);
            var c2 = center + new Vector3(halfSize.x, 0f, -halfSize.z);
            var c3 = center + new Vector3(halfSize.x, 0f, halfSize.z);
            var c4 = center + new Vector3(-halfSize.x, 0f, halfSize.z);
            Gizmos.DrawLine(c1, c2);
            Gizmos.DrawLine(c2, c3);
            Gizmos.DrawLine(c3, c4);
            Gizmos.DrawLine(c4, c1);

            Gizmos.color = initialColor;
        }
    }
}
