using UnityEngine;
using UnityEngine.UI;

namespace NeonBots.UI
{
    [DisallowMultipleComponent, RequireComponent(typeof(Button))]
    public class FullScreenButton : MonoBehaviour
    {
        private Button button;

        private void OnEnable()
        {
            this.button ??= this.GetComponent<Button>();
            this.button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            this.button.onClick.RemoveListener(OnClick);
        }

        private static void OnClick()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if(Screen.fullScreen)
            {
                Screen.fullScreen = false;
            }
            else
            {
                var resolution = Screen.currentResolution;
                Screen.SetResolution(resolution.width, resolution.height, true);
            }
#elif UNITY_WEBGL && !UNITY_EDITOR
            External.Fullscreen();
#else
            Debug.Log("Fullscreen, nothing happens in editor.");
#endif
        }
    }
}
