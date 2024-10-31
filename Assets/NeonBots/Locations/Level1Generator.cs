using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NeonBots.Locations.Constraints;
using UnityEngine;

namespace NeonBots.Locations
{
    public class Level1Generator : LocationGeneratorWfc
    {
        [Header("Level 1")]
        public VoxelTileData startTile;

        public VoxelTileData exitTile;

        private Vector3 startPosition;

        public override Vector3 GetStartPosition() => this.startPosition;

        public override async UniTask Generate()
        {
            if(this.tileSet == default)
            {
                Debug.LogError("No tile set selected");
                return;
            }

            if(this.tiles != default && this.tiles.Count > 0)
            {
                foreach(var tile in this.tiles) Destroy(tile.gameObject);
                this.tiles.Clear();
            }

            this.Cancel();
            this.ProcessSamples();
            this.data = new List<VoxelTileData>[this.locationSize.x, this.locationSize.y, this.locationSize.z];

            // Place start.
            var startPosition = new Vector3Int(Random.Range(1, this.data.GetLength(0) - 1), 0,
                Random.Range(1, this.data.GetLength(2) - 1));

            this.data[startPosition.x, startPosition.y, startPosition.z] = new() { this.startTile };

            // Place exit.
            var exitPosition = new Vector3Int(Random.Range(1, this.data.GetLength(0) - 1), 0,
                Random.Range(1, this.data.GetLength(2) - 1));

            this.data[exitPosition.x, exitPosition.y, exitPosition.z] = new() { this.exitTile };

            // Fill all free positions with samples.
            foreach(var (x, y, z) in this.IterateData())
            {
                if(this.data[x, y, z] != default && this.data[x, y, z].Count > 0) continue;
                var list = this.data[x, y, z] = new();
                list.AddRange(this.workSamples);
            }

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
            await this.Propagate(startPosition);
            await this.Propagate(exitPosition);

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
            this.startPosition = start + (Vector3)startPosition * tileSize;

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
        }
    }
}
