using System;

namespace NeonBots.Locations
{
    [Serializable]
    public class VoxelTileSide
    {
        public int size;

        public int[] data;

        public VoxelTileSide(int size)
        {
            this.size = size;
            this.data = new int[this.size * this.size];
        }

        public void CopyTo(VoxelTileSide other) => this.data.CopyTo(other.data, 0);

        public int[] Mirrored()
        {
            var result = new int[this.data.Length];

            for(var l = 0; l < this.size; l++)
                for(var p = 0; p < this.size; p++)
                    result[l * this.size + p] = this.data[l * this.size + this.size - 1 - p];

            return result;
        }
    }
}
