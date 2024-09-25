using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Managers
{
    public class DebugManager : Manager
    {
        public class Log
        {
            public LogType type;

            public string text;

            public string stackTrace;
        }

        public List<Log> logs = new();

        public event Action<Log> OnLogAdd;

        public LogType status = LogType.Log;

        public event Action<LogType> OnStatusChange;

        public bool chatDebug;

        public event Action<bool> OnChatDebugChange;

        public bool showTouches;

        public event Action<bool> OnShowTouchesChange;

        private void Awake()
        {
            Application.logMessageReceived += this.AddLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= this.AddLog;
        }

        private void AddLog(string text, string stackTrace, LogType type)
        {
            var time = DateTime.Now.ToString("[HH:mm:ss]");
            var log = new Log { type = type, text = $"{time} {text}", stackTrace = stackTrace};
            this.logs.Add(log);
            this.OnLogAdd?.Invoke(log);

            switch(type)
            {
                case LogType.Warning:
                    if(this.status is LogType.Log)
                    {
                        this.status = LogType.Warning;
                        this.OnStatusChange?.Invoke(this.status);
                    }
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if(this.status is LogType.Log or LogType.Warning)
                    {
                        this.status = LogType.Error;
                        this.OnStatusChange?.Invoke(this.status);
                    }
                    break;
            }
        }

        public void SwitchChatDebug(bool state)
        {
            this.chatDebug = state;
            this.OnChatDebugChange?.Invoke(state);
        }

        public void SwitchShowTouches(bool state)
        {
            this.showTouches = state;
            this.OnShowTouchesChange?.Invoke(state);
        }
    }
}
