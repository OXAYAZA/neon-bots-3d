using System;
using UnityEngine;

namespace UIBlock
{
    public static class Gaussian
    {
        public const ushort MaxKernelRadius = 18;

        public static float Function(int x, int y, float sigma)
        {
            var sigmaSqr2 = 2f * sigma * sigma;
            return Mathf.Exp(-(x * x + y * y) / sigmaSqr2) / (Mathf.PI * sigmaSqr2);
        }

        public static float[] GenerateKernelFlatQuad(byte radius = 9, float sigma = 5f, bool normalize = true)
        {
            if(radius > MaxKernelRadius)
                throw new ArgumentOutOfRangeException(nameof(radius), radius,
                    $"Kernel radius must be lower or equal {MaxKernelRadius}.");

            radius++;
            var kernel = new float[radius * radius];
            var sum = 0f;

            for(var y = 0; y < radius; y++)
            {
                for(var x = 0; x < radius; x++)
                {
                    var val = kernel[y * radius + x] = Function(x, y, sigma);
                    sum += y == 0 || x == 0 ? val * 2f : val * 4f;
                }
            }

            if(normalize) for(var i = 0; i < kernel.Length; i++) kernel[i] = kernel[i] / sum;

            return kernel;
        }
    }
}
