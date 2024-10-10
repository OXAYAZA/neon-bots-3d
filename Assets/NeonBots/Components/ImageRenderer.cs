using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NeonBots.Editor
{
    [RequireComponent(typeof(Transform), typeof(Camera))]
    public class ImageRenderer : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int resolution = new(512, 512);

#if UNITY_EDITOR
        [ContextMenu("Render Image")]
        public void Render()
        {
            var currentRT = RenderTexture.active;
            var targetTexture = RenderTexture.GetTemporary(this.resolution.x, this.resolution.y, 0, RenderTextureFormat.ARGB32);
            var camera = this.GetComponent<Camera>();
            camera.targetTexture = targetTexture;
            RenderTexture.active = camera.targetTexture;
            camera.Render();
            
            var image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
            var rect = new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height);
            image.ReadPixels(rect, 0, 0);
            image.Apply();

            camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(targetTexture);
            RenderTexture.active = currentRT;

            if(!AssetDatabase.IsValidFolder("Assets/RenderedImages"))
                AssetDatabase.CreateFolder("Assets", "RenderedImages");

            var bytes = image.EncodeToPNG();
            var fullPath = $"Assets/RenderedImages/{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.png";
            File.WriteAllBytes(fullPath, bytes);
        }
#endif
    }
}
