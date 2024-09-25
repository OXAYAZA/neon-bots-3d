using UnityEngine;

namespace UIBlock
{
    public static class GradientExtension
    {
        public static Hash128 GetHash(this Gradient gradient)
        {
            var hash = new Hash128();
            hash.Append(gradient.alphaKeys);
            hash.Append(gradient.colorKeys);
            hash.Append((int)gradient.colorSpace);
            hash.Append((int)gradient.mode);
            return hash;
        }
    }
}