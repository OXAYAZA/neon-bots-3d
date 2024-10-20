using System;

namespace NeonBots.Locations
{
    [Serializable]
    public class VoxelTileSide
    {
        public int size;

        public int[] data;

        private int[] mirrored;

        public VoxelTileSide(int size)
        {
            this.size = size;
            this.data = new int[this.size * this.size];
        }

        public void CopyTo(VoxelTileSide other) => this.data.CopyTo(other.data, 0);

        public int[] Mirrored()
        {
            if(this.mirrored != default) return this.mirrored;

            this.mirrored = new int[this.data.Length];

            for(var l = 0; l < this.size; l++)
                for(var p = 0; p < this.size; p++)
                    this.mirrored[l * this.size + p] = this.data[l * this.size + this.size - 1 - p];

            return this.mirrored;
        }
    }
}
