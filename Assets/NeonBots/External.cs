using System.Runtime.InteropServices;

namespace NeonBots
{
    public static class External
    {
        [DllImport("__Internal")]
        public static extern void Fullscreen();
    }
}
