using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeonBots.Locations
{
    public class TileGenerator : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int locationSize = new(10, 10);

        [SerializeField]
        private float tileSize = 1f;

        public List<VoxelTile> samples;

        private VoxelTile[,] location;

        private Vector3 center;

        private Vector3 startPosition;

        private bool isReady;

        private void Start()
        {
            var initialCount = this.samples.Count;
            
            for(var i = 0; i < initialCount; i++)
            {
                var sample = this.samples[i];
            
                switch(sample.rotations)
                {
                    case VoxelTile.Rotations.None:
                        break;
                    case VoxelTile.Rotations.Two:
                    {
                        sample.weight *= 0.5f;
                        var clone = Instantiate(sample,
                            sample.transform.position + Vector3.back * this.tileSize * 2f,
                            sample.transform.rotation);
                        clone.Rotate90();
                        this.samples.Add(clone);
                        break;
                    }
                    case VoxelTile.Rotations.Four:
                    {
                        sample.weight *= 0.25f;
                        var clone = Instantiate(sample,
                            sample.transform.position + Vector3.back * this.tileSize * 2f,
                            sample.transform.rotation);
                        clone.Rotate90();
                        this.samples.Add(clone);

                        clone = Instantiate(clone,
                            clone.transform.position + Vector3.back * this.tileSize * 2f,
                            clone.transform.rotation);
                        clone.Rotate90();
                        this.samples.Add(clone);

                        clone = Instantiate(clone,
                            clone.transform.position + Vector3.back * this.tileSize * 2f,
                            clone.transform.rotation);
                        clone.Rotate90();
                        this.samples.Add(clone);

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            this.isReady = true;
        }

        private void Update()
        {
            if(!this.isReady) return;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(this.location != default)
                    foreach(var tile in this.location)
                        if(tile != default) Destroy(tile.gameObject);

                this.Generate();
            }
        }

        private void Generate()
        {
            // Here, empty tiles are added as fields.
            this.location = new VoxelTile[this.locationSize.x + 2, this.locationSize.y + 2];
            var size = new Vector3(this.locationSize.x * this.tileSize, 0f, this.locationSize.y * this.tileSize);
            this.startPosition = this.center - size * 0.5f - new Vector3(this.tileSize, 0f, this.tileSize) * 0.5f;

            for(var y = 1; y < this.location.GetLength(1) - 1; y++)
                for(var x = 1; x < this.location.GetLength(0) - 1; x++)
                    this.PlaceTile(x, y);
        }

        private void PlaceTile(int x, int y)
        {
            var filteredSamples = this.samples.Where(tile =>
                this.CanAppendTile(this.location[x, y - 1], tile, VoxelTile.Side.Front) &&
                this.CanAppendTile(this.location[x + 1, y], tile, VoxelTile.Side.Left) &&
                this.CanAppendTile(this.location[x, y + 1], tile, VoxelTile.Side.Back) &&
                this.CanAppendTile(this.location[x - 1, y], tile, VoxelTile.Side.Right)).ToList();

            if(filteredSamples.Count == 0) return;

            var resultSample = this.RandomTile(filteredSamples);
            var position = this.startPosition + new Vector3(x, 0f, y) * this.tileSize;
            var newTile = Instantiate(resultSample, position, resultSample.transform.rotation);
            this.location[x, y] = newTile;
        }

        private bool CanAppendTile(VoxelTile target, VoxelTile comparable, VoxelTile.Side side)
        {
            if(target == default) return true;

            return side switch
            {
                VoxelTile.Side.Back =>
                    target.sideBack.SequenceEqual(this.MirrorSide(comparable.sideFront, comparable.tileSize)),
                VoxelTile.Side.Right =>
                    target.sideRight.SequenceEqual(this.MirrorSide(comparable.sideLeft, comparable.tileSize)),
                VoxelTile.Side.Front =>
                    target.sideFront.SequenceEqual(this.MirrorSide(comparable.sideBack, comparable.tileSize)),
                VoxelTile.Side.Left =>
                    target.sideLeft.SequenceEqual(this.MirrorSide(comparable.sideRight, comparable.tileSize)),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }

        private int[] MirrorSide(int[] side, int size)
        {
            var result = new int[side.Length];

            for(var l = 0; l < size; l++)
                for(var p = 0; p < size; p++)
                    result[l * size + p] = side[l * size + size - 1 - p];

            return result;
        }

        private VoxelTile RandomTile(List<VoxelTile> tiles)
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
            var size = new Vector3(this.locationSize.x * this.tileSize, 0f, this.locationSize.y * this.tileSize);
            var halfSize = size * 0.5f;
            var c1 = this.center + new Vector3(-halfSize.x, 0f, -halfSize.z);
            var c2 = this.center + new Vector3(halfSize.x, 0f, -halfSize.z);
            var c3 = this.center + new Vector3(halfSize.x, 0f, halfSize.z);
            var c4 = this.center + new Vector3(-halfSize.x, 0f, halfSize.z);
            Gizmos.DrawLine(c1, c2);
            Gizmos.DrawLine(c2, c3);
            Gizmos.DrawLine(c3, c4);
            Gizmos.DrawLine(c4, c1);

            Gizmos.color = initialColor;
        }
    }
}
