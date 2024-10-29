using System;
using System.Collections.Generic;
using System.Linq;
using NeonBots.Screens;
using UnityEngine;

namespace NeonBots.Managers
{
    public class UIManager : Manager
    {
        public class PathItem
        {
            public readonly IScreen screen;

            public Dictionary<string, object> data;

            public PathItem(IScreen screen, Dictionary<string, object> data = null)
            {
                this.screen = screen;
                this.data = data;
            }
        }

        public int baseDpi = 96;

        public int baseSize = 360;

        public int maxSize = 800;

        public GameObject rootObject;

        public LoadingScreen overlay;

        public float ScaleFactor { get; private set; }

        public Vector2 ScaledSize { get; private set; }

        public Rect ScaledSafeArea { get; private set; }

        public List<PathItem> path;

        private Dictionary<Type, UIScreen> screens;

        private Vector2 resolution;

        private bool fullscreen;

        public event Action OnResize;

        private void Awake()
        {
            this.screens = new();
            this.path = new();

            if(this.rootObject == default) this.rootObject = this.gameObject;

            foreach(var screen in this.rootObject.GetComponentsInChildren<UIScreen>(true))
                this.screens.Add(screen.GetType(), screen);

            this.resolution = new(Screen.width, Screen.height);
            this.fullscreen = Screen.fullScreen;
            this.Resize();
        }

        private void Update()
        {
            var resolution = new Vector2(Screen.width, Screen.height);
            if(this.resolution == resolution && this.fullscreen == Screen.fullScreen) return;
            this.resolution = resolution;
            this.fullscreen = Screen.fullScreen;
            this.Resize();
            this.OnResize?.Invoke();
        }

        public void Resize()
        {
            var size = new Vector2(Screen.width, Screen.height);
            var dpiFactor = Screen.dpi / this.baseDpi;
            var dpiSize = size / dpiFactor;
            var side = Mathf.Min(dpiSize.x, dpiSize.y);
            var sideFactor = side < this.baseSize
                ? side / this.baseSize
                : side > this.maxSize
                    ? side / this.maxSize
                    : 1f;

            this.ScaleFactor = dpiFactor * sideFactor;
            this.ScaledSize = size / this.ScaleFactor;
            var scaledSafePosition = Screen.safeArea.position / this.ScaleFactor;
            var scaledSafeSize = Screen.safeArea.size / this.ScaleFactor;
            this.ScaledSafeArea = new(scaledSafePosition.x, scaledSafePosition.y, scaledSafeSize.x, scaledSafeSize.y);
        }

        public T GetScreen<T>() where T : UIScreen
        {
            if(!this.screens.TryGetValue(typeof(T), out var screen))
                throw new InvalidOperationException($"No screen with type \"{typeof(T)}\"");

            return (T)screen;
        }

        public void Open(IScreen screen)
        {
            if(this.path.Count > 0)
            {
                var currentItem = this.path.Last();
                var currentScreen = currentItem.screen;

                if(currentScreen == screen) return;

                if(currentScreen.IsPopup())
                {
                    this.Back(true);
                    currentItem = this.path.Last();
                    currentScreen = currentItem.screen;
                }

                currentItem.data = currentScreen.GetState();

                if(!screen.IsPopup()) currentScreen.Switch(false);
            }

            this.path.Add(new(screen));
            screen.Switch();
        }

        public void Close(IScreen screen)
        {
            var pathItem = this.path.FirstOrDefault(item => item.screen == screen);
            if(pathItem != default) this.path.Remove(pathItem);
            screen.Switch(false);
        }

        public void Back(bool force)
        {
            if(this.path.Count <= 1) return;

            var currentScreen = this.path.Last().screen;
            if(currentScreen.IsBlockBack() && !force) return;

            this.path.RemoveAt(this.path.Count - 1);
            currentScreen.Switch(false);
            
            var targetItem = this.path.Last();
            targetItem.screen.SetState(targetItem.data);
            targetItem.screen.Switch();
        }

        public void Back() => this.Back(false);

        public void GoTo(IScreen screen)
        {
            var item = this.path.LastOrDefault(item => item.screen == screen);
            if(item != null) while(this.path.Last().screen != screen && this.path.Count != 0) this.Back();
            else this.Open(screen);
        }

        public void SwitchOverlay(bool state) => this.overlay.Switch(state);

        public void ClearHistory() => this.path.Clear();
    }
}
