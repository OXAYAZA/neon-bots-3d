using TMPro;
using UnityEngine;

namespace NeonBots.UI
{
    public class RamStatus : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text statusText;

        private void OnEnable()
        {
            #if !UNITY_ANDROID || UNITY_EDITOR
            this.gameObject.SetActive(false);
            return;
            #endif

            this.Refresh();
        }

        private void Update() => this.Refresh();

        private void Refresh() => this.statusText.text = $"💽 {this.GetAvailableRam():000} MB";

        private long GetAvailableRam()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            var manager = activity.Call<AndroidJavaObject>("getSystemService", "activity");
            var memoryInfo = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo");
            manager.Call("getMemoryInfo", memoryInfo);
            return memoryInfo.Get<long>("availMem") / 1024 / 1024;
            #else
            return 0;
            #endif
        }
    }
}
