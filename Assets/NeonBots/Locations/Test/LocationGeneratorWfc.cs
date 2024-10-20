using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeonBots.Locations
{
    public class LocationGeneratorWfc : MonoBehaviour
    {
        public Vector2Int locationSize = new(10, 10);

        public float tileSize = 1f;

        public List<VoxelTileData> samples;

        public List<VoxelTile> tiles;

        private List<VoxelTileData> workSamples;

        private List<VoxelTileData>[,] data;

        private bool isReady;

        private CancellationTokenSource cts;

        private Vector2Int gizmo1;

        private Stack<Vector2Int> gizmo2;

        private void Start()
        {
            this.ProcessSamples();
            this.isReady = true;
            this.Regenerate().Forget();
        }

        [ContextMenu("Regenerate")]
        private async UniTask Regenerate()
        {
            if(!Application.isPlaying || !this.isReady) return;
            
            if(this.tiles.Count > 0)
            {
                foreach(var tile in this.tiles) Destroy(tile.gameObject);
                this.tiles.Clear();
            }

            this.Cancel();
            await this.Generate();

            var data = this.data;
            this.data = null;
            var size = new Vector3(this.locationSize.x * this.tileSize, 0f, this.locationSize.y * this.tileSize);
            var offset = new Vector3(this.tileSize * 0.5f, 0f, this.tileSize * 0.5f);
            var start = this.transform.position - size * 0.5f;

            for(var y = 0; y < data.GetLength(1); y++)
            {
                for(var x = 0; x < data.GetLength(0); x++)
                {
                    var tileData = data[x, y];
                    if(tileData == null || tileData.Count == 0) continue;
                    var position = start + offset + new Vector3(x, 0f, y) * this.tileSize;
                    var sample = tileData[0];
                    var tile = Instantiate(sample.tile, position, Quaternion.Euler(sample.rotation));
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
                        clone.name = sample.name;
                        this.workSamples.Add(clone);
                        break;
                    }
                    case VoxelTileRotations.Two:
                    {
                        var weight = sample.weight * 0.5f;
                        var clone = Instantiate(sample);
                        clone.name = sample.name;
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate90();
                        clone.name = $"{sample.name}90";
                        clone.weight = weight;
                        this.workSamples.Add(clone);
                        break;
                    }
                    case VoxelTileRotations.Four:
                    {
                        var weight = sample.weight * 0.25f;
                        var clone = Instantiate(sample);
                        clone.name = sample.name;
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate90();
                        clone.name = $"{sample.name}90";
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate180();
                        clone.name = $"{sample.name}180";
                        clone.weight = weight;
                        this.workSamples.Add(clone);

                        clone = Instantiate(sample);
                        clone.Rotate270();
                        clone.name = $"{sample.name}270";
                        clone.weight = weight;
                        this.workSamples.Add(clone);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async UniTask Generate()
        {
            this.data = new List<VoxelTileData>[this.locationSize.x, this.locationSize.y];

            // Fill all positions with samples.
            for(var y = 0; y < this.data.GetLength(1); y++)
            {
                for(var x = 0; x < this.data.GetLength(0); x++)
                {
                    var list = this.data[x, y] = new();
                    list.AddRange(this.workSamples);
                }
            }

            // Start generation.
            while(!this.IsCollapsed() && !this.cts.Token.IsCancellationRequested)
            {
                var position = this.GetMinEntropyPosition();
                this.Collapse(position);
                this.Propagate(position);
                await UniTask.Yield(this.cts.Token);
            }
        }

        private bool IsCollapsed()
        {
            for(var y = 0; y < this.data.GetLength(1); y++)
                for(var x = 0; x < this.data.GetLength(0); x++)
                    if(this.data[x, y].Count > 1) return false;

            return true;
        }

        private Vector2Int GetMinEntropyPosition()
        {
            var entropy = this.workSamples.Count;
            var none = new Vector2Int(-1, -1);
            var position = none;
            var list = new List<Vector2Int>();

            for(var y = 0; y < this.data.GetLength(1); y++)
            {
                for(var x = 0; x < this.data.GetLength(0); x++)
                {
                    var count = this.data[x, y].Count;

                    if(count > 1)
                    {
                        if(count < entropy)
                        {
                            entropy = count;
                            position = new(x, y);
                        }
                        else
                        {
                            list.Add(new(x, y));
                        }
                        
                    }
                }
            }

            if(position == none) position = list[Random.Range(0, list.Count)];
            return position;
        }

        private void Collapse(Vector2Int pos)
        {
            var tiles = this.data[pos.x, pos.y];
            if(tiles.Count == 0) return;

            this.data[pos.x, pos.y] = new();
            var weights = tiles.Select(item => item.weight).ToList();
            var value = Random.Range(0f, weights.Sum());
            var sum = 0f;

            for(var i = 0; i < weights.Count; i++)
            {
                sum += weights[i];

                if(value < sum)
                {
                    this.data[pos.x, pos.y].Add(tiles[i]);
                    break;
                }
            }
        }

        private void Propagate(Vector2Int position)
        {
            this.gizmo1 = position;
            var stack = new Stack<Vector2Int>();
            this.gizmo2 = stack;
            stack.Push(position);

            while(stack.Count > 0 && !this.cts.Token.IsCancellationRequested)
            {
                var currentPosition = stack.Pop();
                var currentSamples = this.data[currentPosition.x, currentPosition.y];

                foreach(var (neighborPosition, side) in this.ValidPositions(currentPosition))
                {
                    var possibleSamples = this.PossibleSamples(currentSamples, side);
                    var neighborSamples = this.data[neighborPosition.x, neighborPosition.y];

                    foreach(var neighborSample in neighborSamples.ToList())
                    {
                        if(possibleSamples.Contains(neighborSample)) continue;
                        neighborSamples.Remove(neighborSample);
                        if(stack.All(pos => pos != neighborPosition)) stack.Push(neighborPosition);
                    }
                }
            }
        }

        private List<(Vector2Int, VoxelTile.Side)> ValidPositions(Vector2Int position)
        {
            var neighbourPositions = new List<(Vector2Int, VoxelTile.Side)>();

            var back = position + Vector2Int.down;
            var right = position + Vector2Int.right;
            var front = position + Vector2Int.up;
            var left = position + Vector2Int.left;

            if(back.y >= 0 && back.y < this.data.GetLength(1))
                neighbourPositions.Add((back, VoxelTile.Side.Back));

            if(right.x >= 0 && right.x < this.data.GetLength(0))
                neighbourPositions.Add((right, VoxelTile.Side.Right));

            if(front.y >= 0 && front.y < this.data.GetLength(1))
                neighbourPositions.Add((front, VoxelTile.Side.Front));

            if(left.x >= 0 && left.x < this.data.GetLength(0))
                neighbourPositions.Add((left, VoxelTile.Side.Left));

            return neighbourPositions;
        }

        private List<VoxelTileData> PossibleSamples(List<VoxelTileData> currentSamples, VoxelTile.Side side)
        {
            var possibleSamples = new List<VoxelTileData>();

            foreach(var currentSample in currentSamples)
                foreach(var workSample in this.workSamples)
                    if(currentSample.CompareSide(side, workSample) && !possibleSamples.Contains(workSample))
                        possibleSamples.Add(workSample);

            return possibleSamples;
        }

        [ContextMenu("Cancel generation")]
        private void Cancel()
        {
            if(!Application.isPlaying) return;
            this.cts?.Cancel();
            this.cts = new();
        }

        private void OnDrawGizmos()
        {
            var initialColor = Gizmos.color;

            var center = this.transform.position;
            var size = new Vector3(this.locationSize.x * this.tileSize, 0f, this.locationSize.y * this.tileSize);
            var halfSize = size * 0.5f;
            var c1 = center + new Vector3(-halfSize.x, 0f, -halfSize.z);
            var c2 = center + new Vector3(halfSize.x, 0f, -halfSize.z);
            var c3 = center + new Vector3(halfSize.x, 0f, halfSize.z);
            var c4 = center + new Vector3(-halfSize.x, 0f, halfSize.z);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(c1, c2);
            Gizmos.DrawLine(c2, c3);
            Gizmos.DrawLine(c3, c4);
            Gizmos.DrawLine(c4, c1);

            if(!Application.isPlaying || this.data == default) return;

            var tileSize = new Vector3(this.tileSize * 0.9f, 0.2f, this.tileSize * 0.9f);
            var offset = new Vector3(this.tileSize * 0.5f, 0f, this.tileSize * 0.5f);

            for(var z = 0; z < this.data.GetLength(1); z++)
            {
                for(var x = 0; x < this.data.GetLength(0); x++)
                {
                    if(this.data[x, z].Count > 1)
                    {
                        Gizmos.color = new(1f, 1f, 0f, 0.5f);
                        Gizmos.DrawCube(c1 + new Vector3(x, 0f, z) + offset,
                            tileSize * (1f - (float)this.data[x, z].Count / (this.workSamples.Count + 3)));
                    }
                    else if(this.data[x, z].Count == 1)
                    {
                        Gizmos.color = new(0f, 1f, 0f, 0.5f);
                        Gizmos.DrawCube(c1 + new Vector3(x, 0f, z) + offset, tileSize);
                    }
                    else
                    {
                        Gizmos.color = new(1f, 0f, 0f, 0.5f);
                        Gizmos.DrawCube(c1 + new Vector3(x, 0f, z) + offset, tileSize);
                    }
                    
                }
            }


            Gizmos.color = new(0f, 0f, 1f, 0.5f);

            if(this.gizmo2 != default && this.gizmo2.Count > 0)
            {
                foreach(var pos in this.gizmo2)
                {
                    var position2 = c1 + new Vector3(pos.x, 0f, pos.y) + offset;
                    Gizmos.DrawCube(position2, tileSize);
                }
            }

            var position1 = c1 + new Vector3(this.gizmo1.x, 0f, this.gizmo1.y) + offset;
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(position1, tileSize);

            Gizmos.color = initialColor;
        }
    }
}
