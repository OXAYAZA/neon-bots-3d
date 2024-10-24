using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeonBots.Locations.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeonBots.Locations
{
    public class LocationGeneratorWfc : MonoBehaviour
    {
        public Vector3Int locationSize = new(10, 10, 10);

        public Transform initialTilesContainer;

        public TileSet tileSet;

        private List<VoxelTile> tiles;

        private List<VoxelTile> initialTiles;

        private List<VoxelTileData> workSamples;

        private List<VoxelTileData>[,,] data;

        private CancellationTokenSource cts;

        private Vector3Int gizmo1;

        private Stack<Vector3Int> gizmo2;

        private void Start()
        {
            this.Generate().Forget();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space)) this.Generate().Forget();
        }

        private IEnumerable<(int, int, int)> IterateData()
        {
            for(var y = 0; y < this.data.GetLength(1); y++)
                for(var z = 0; z < this.data.GetLength(2); z++)
                    for(var x = 0; x < this.data.GetLength(0); x++)
                        yield return (x, y, z);
        }

        [ContextMenu("Generate")]
        private async UniTask Generate()
        {
            if(!Application.isPlaying) return;

            if(this.tileSet == default)
            {
                Debug.LogError("No tile set selected");
                return;
            }

            Debug.Log($"Location generation started: {DateTime.UtcNow}");

            if(this.tiles != default && this.tiles.Count > 0)
            {
                foreach(var tile in this.tiles) Destroy(tile.gameObject);
                this.tiles.Clear();
            }

            this.Cancel();
            this.ProcessSamples();
            this.data = new List<VoxelTileData>[this.locationSize.x, this.locationSize.y, this.locationSize.z];
            this.GetInitialTiles();

            // Fill all free positions with samples.
            foreach(var (x, y, z) in this.IterateData())
            {
                if(this.data[x, y, z] != default && this.data[x, y, z].Count > 0) continue;
                var list = this.data[x, y, z] = new();
                list.AddRange(this.workSamples);
            }

            // Find preset samples.
            var initialPositions = new List<Vector3Int>();

            foreach(var (x, y, z) in this.IterateData())
                if(this.data[x, y, z] != default && this.data[x, y, z].Count == 1)
                    initialPositions.Add(new(x, y, z));

            // Apply initial constraints.
            // TODO: Possible requires propagation.
            foreach(var (x, y, z) in this.IterateData())
            {
                this.gizmo1 = new(x, y, z);
                var samples = this.data[x, y, z];

                foreach(var sample in samples.ToList())
                {
                    foreach(var constraint in sample.constraints)
                    {
                        if(!constraint.GetType().IsSubclassOf(typeof(InitialConstraint))) continue;
                        if(((InitialConstraint)constraint).Check(this.data, new(x, y, z))) continue;
                        samples.Remove(sample);
                    }
                }
            }

            // Propagate influence of initial samples.
            foreach(var initialPosition in initialPositions)
                await this.Propagate(initialPosition);

            // Start generation.
            while(!this.IsCollapsed() && !this.cts.Token.IsCancellationRequested)
            {
                var position = this.GetMinEntropyPosition();
                this.Collapse(position);
                await this.Propagate(position);
            }

            // Fill location.
            var data = this.data;
            this.data = null;
            this.tiles = new();

            var tileSize = this.tileSet.tileSize * this.tileSet.tileScale;
            var size = new Vector3(
                this.locationSize.x * tileSize,
                this.locationSize.y * tileSize,
                this.locationSize.z * tileSize
            );
            var halfSize = size * 0.5f;
            var offset = Vector3.one * (tileSize * 0.5f);
            var start = this.transform.position - halfSize + offset;

            for(var y = 0; y < data.GetLength(1); y++)
            {
                for(var z = 0; z < data.GetLength(2); z++)
                {
                    for(var x = 0; x < data.GetLength(0); x++)
                    {
                        var tileData = data[x, y, z];
                        if(tileData == null || tileData.Count == 0) continue;
                        var position = start + new Vector3(x, y, z) * tileSize;
                        var sample = tileData[0];
                        var tile = Instantiate(sample.tile, position, Quaternion.Euler(sample.rotation), this.transform);
                        tile.transform.localScale = Vector3.one * this.tileSet.tileScale;
                        tile.name = sample.tile.name;
                        this.tiles.Add(tile);
                    }
                }
            }

            Debug.Log($"Location generation ended: {DateTime.UtcNow}");
        }

        private void ProcessSamples()
        {
            this.workSamples = new();

            foreach(var sample in this.tileSet.samples)
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

                        var clone1 = Instantiate(sample);
                        clone1.name = sample.name;
                        clone1.weight = weight;
                        this.workSamples.Add(clone1);

                        var clone2 = Instantiate(sample);
                        clone2.Rotate90();
                        clone2.name = $"{sample.name}-90";
                        clone2.weight = weight;
                        this.workSamples.Add(clone2);
                        break;
                    }
                    case VoxelTileRotations.Four:
                    {
                        var weight = sample.weight * 0.25f;

                        var clone1 = Instantiate(sample);
                        clone1.name = sample.name;
                        clone1.weight = weight;
                        this.workSamples.Add(clone1);

                        var clone2 = Instantiate(sample);
                        clone2.Rotate90();
                        clone2.name = $"{sample.name}-90";
                        clone2.weight = weight;
                        this.workSamples.Add(clone2);

                        var clone3 = Instantiate(sample);
                        clone3.Rotate180();
                        clone3.name = $"{sample.name}-180";
                        clone3.weight = weight;
                        this.workSamples.Add(clone3);

                        var clone4 = Instantiate(sample);
                        clone4.Rotate270();
                        clone4.name = $"{sample.name}-270";
                        clone4.weight = weight;
                        this.workSamples.Add(clone4);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void GetInitialTiles()
        {
            if(this.initialTilesContainer == default) return;

            this.initialTilesContainer.gameObject.SetActive(true);

            var tileSize = this.tileSet.tileSize * this.tileSet.tileScale;
            var size = new Vector3(
                this.locationSize.x * tileSize,
                this.locationSize.y * tileSize,
                this.locationSize.z * tileSize
            );
            var halfSize = size * 0.5f;
            var offset = Vector3.one * (tileSize * 0.5f);
            var start = this.transform.position - halfSize + offset;

            this.initialTiles = this.initialTilesContainer.GetComponentsInChildren<VoxelTile>().ToList();

            foreach(var item in this.initialTiles)
            {
                var position = Vector3Int.FloorToInt((item.transform.position - start) / tileSize);
                var sample = this.workSamples.FirstOrDefault(sample =>
                    sample.tile.name == item.name && sample.rotation == item.transform.rotation.eulerAngles);

                if(sample != default
                   && position is { x: >= 0, y: >= 0, z: >= 0 }
                   && position.x < this.locationSize.x
                   && position.y < this.locationSize.y
                   && position.z < this.locationSize.z)
                {
                    this.data[position.x, position.y, position.z] = new() { sample };
                }
                else
                {
                    Debug.Log($"Failed existing tile: {item.name}");
                }
            }

            this.initialTilesContainer.gameObject.SetActive(false);
        }

        private bool IsCollapsed()
        {
            foreach(var (x, y, z) in this.IterateData())
                if(this.data[x, y, z].Count > 1) return false;

            return true;
        }

        private Vector3Int GetMinEntropyPosition()
        {
            var entropy = this.workSamples.Count;
            var none = new Vector3Int(-1, -1, -1);
            var position = none;
            var list = new List<Vector3Int>();

            foreach(var (x, y, z) in this.IterateData())
            {
                var count = this.data[x, y, z].Count;

                if(count > 1)
                {
                    if(count < entropy)
                    {
                        entropy = count;
                        position = new(x, y, z);
                    }
                    else
                    {
                        list.Add(new(x, y, z));
                    }
                }
            }

            if(position == none) position = list[Random.Range(0, list.Count)];
            return position;
        }

        private void Collapse(Vector3Int pos)
        {
            var samples = this.data[pos.x, pos.y, pos.z];
            if(samples.Count == 0) return;

            this.data[pos.x, pos.y, pos.z] = new();
            var weights = samples.Select(item => item.weight).ToList();
            var value = Random.Range(0f, weights.Sum());
            var sum = 0f;

            for(var i = 0; i < weights.Count; i++)
            {
                sum += weights[i];

                if(value < sum)
                {
                    this.data[pos.x, pos.y, pos.z].Add(samples[i]);
                    break;
                }
            }
        }

        private async UniTask Propagate(Vector3Int startPosition)
        {
            this.gizmo1 = startPosition;
            var stack = new Stack<Vector3Int>();
            this.gizmo2 = stack;
            stack.Push(startPosition);

            while(stack.Count > 0 && !this.cts.Token.IsCancellationRequested)
            {
                var currentPosition = stack.Pop();

                foreach(var (neighborPosition, side) in this.ValidPositions(currentPosition))
                {
                    var neighbors = this.data[neighborPosition.x, neighborPosition.y, neighborPosition.z];
                    if(neighbors.Count <= 1) continue;
                    var possibleNeighbors = this.PossibleSamples(startPosition, currentPosition, neighborPosition, side);

                    foreach(var neighbor in neighbors.ToList())
                    {
                        if(possibleNeighbors.Contains(neighbor)) continue;
                        neighbors.Remove(neighbor);
                        if(stack.All(pos => pos != neighborPosition)) stack.Push(neighborPosition);
                    }
                }

                await UniTask.Yield(this.cts.Token);
            }
        }

        private List<(Vector3Int, VoxelTile.Side)> ValidPositions(Vector3Int position)
        {
            var neighbourPositions = new List<(Vector3Int, VoxelTile.Side)>();

            var back = position + Vector3Int.back;
            var right = position + Vector3Int.right;
            var front = position + Vector3Int.forward;
            var left = position + Vector3Int.left;
            var top = position + Vector3Int.up;
            var bottom = position + Vector3Int.down;

            if(back.z >= 0 && back.z < this.data.GetLength(2))
                neighbourPositions.Add((back, VoxelTile.Side.Back));

            if(right.x >= 0 && right.x < this.data.GetLength(0))
                neighbourPositions.Add((right, VoxelTile.Side.Right));

            if(front.z >= 0 && front.z < this.data.GetLength(2))
                neighbourPositions.Add((front, VoxelTile.Side.Front));

            if(left.x >= 0 && left.x < this.data.GetLength(0))
                neighbourPositions.Add((left, VoxelTile.Side.Left));

            if(top.y >= 0 && top.y < this.data.GetLength(1))
                neighbourPositions.Add((top, VoxelTile.Side.Top));

            if(bottom.y >= 0 && bottom.y < this.data.GetLength(1))
                neighbourPositions.Add((bottom, VoxelTile.Side.Bottom));

            return neighbourPositions;
        }

        private List<VoxelTileData> PossibleSamples(Vector3Int startPosition, Vector3Int currentPosition, Vector3Int neighborPosition, VoxelTile.Side side)
        {
            var constrainingSample = this.data[startPosition.x, startPosition.y, startPosition.z][0];
            var targetSamples = this.data[currentPosition.x, currentPosition.y, currentPosition.z];
            var possibleSamples = new List<VoxelTileData>();

            foreach(var currentSample in targetSamples)
                foreach(var workSample in this.workSamples)
                    if(currentSample.CompareSide(side, workSample) && !possibleSamples.Contains(workSample))
                        possibleSamples.Add(workSample);

            if(constrainingSample != default && neighborPosition != startPosition)
            {
                foreach(var constraint in constrainingSample.constraints)
                {
                    if(!constraint.GetType().IsSubclassOf(typeof(ActiveConstraint))) continue;

                    var possibleSamples2 = this.workSamples.Where(sample =>
                        ((ActiveConstraint)constraint).Apply(sample, neighborPosition, startPosition));

                    possibleSamples = possibleSamples.Where(sample => possibleSamples2.Contains(sample)).ToList();
                }
            }

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

            if(this.tileSet == default) return;

            var center = this.transform.position;
            var tileSize = this.tileSet.tileSize * this.tileSet.tileScale;
            var size = new Vector3(
                this.locationSize.x * tileSize,
                this.locationSize.y * tileSize,
                this.locationSize.z * tileSize
            );
            var halfSize = size * 0.5f;
            var offset = Vector3.one * (tileSize * 0.5f);
            var start = center - halfSize + offset;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);
            Gizmos.DrawSphere(center - halfSize, 0.1f);

            Gizmos.color = initialColor;

            if(!Application.isPlaying || this.data == default) return;
            
            var selectorSize = Vector3.one * tileSize * 0.9f;
            var sampleSize = Vector3.one * tileSize * 0.8f;
            
            foreach(var (x, y, z) in this.IterateData())
            {
                if(this.data[x, y, z] == default) continue;
            
                if(this.data[x, y, z].Count > 1)
                {
                    Gizmos.color = new(1f, 1f, 0f, 0.5f);
                    Gizmos.DrawCube(start + new Vector3(x, y, z) * tileSize,
                        sampleSize * (1f - (float)this.data[x, y, z].Count / (this.workSamples.Count + 3)));
                }
                else if(this.data[x, y, z].Count == 1)
                {
                    Gizmos.color = new(0f, 1f, 0f, 0.5f);
                    Gizmos.DrawCube(start + new Vector3(x, y, z) * tileSize, sampleSize);
                }
                else
                {
                    Gizmos.color = new(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(start + new Vector3(x, y, z) * tileSize, sampleSize);
                }
            }
            
            if(this.gizmo2 != default && this.gizmo2.Count > 0)
            {
                Gizmos.color = new(0f, 0f, 1f, 0.5f);
            
                foreach(var pos in this.gizmo2)
                    Gizmos.DrawCube(start + new Vector3(pos.x, pos.y, pos.z) * tileSize, selectorSize);
            }
            
            Gizmos.color = new(0f, 1f, 1f, 0.5f);
            Gizmos.DrawCube(start + new Vector3(this.gizmo1.x, this.gizmo1.y, this.gizmo1.z) * tileSize, selectorSize);
        }
    }
}
