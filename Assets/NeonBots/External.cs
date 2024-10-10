using System.Runtime.InteropServices;

namespace NeonBots
{
    public static class External
    {
        [DllImport("__Internal")]
        public static extern void Fullscreen();

        [DllImport("__Internal")]
        public static extern bool IsMobile();

        [DllImport("__Internal")]
        public static extern void ShowPopup(string msg, string type);
    }
}
