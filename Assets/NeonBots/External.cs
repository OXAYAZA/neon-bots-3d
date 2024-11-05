#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace NeonBots
{
    public static class External
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void Fullscreen();
#else
        public static void Fullscreen() {}
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern bool IsMobile();
#else
        public static bool IsMobile() => false;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void ShowPopup(string msg, string type);
#else
        public static void ShowPopup(string msg, string type) {}
#endif
    }
}
