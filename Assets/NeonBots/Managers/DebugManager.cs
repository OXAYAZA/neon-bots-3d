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

        public LogType Status { get; private set; } = LogType.Log;

        public event Action<Log> OnLogAdd;

        public event Action<LogType> OnStatusChange;

        private void Awake() => Application.logMessageReceived += this.AddLog;

        private void OnDestroy() => Application.logMessageReceived -= this.AddLog;

        private void AddLog(string text, string stackTrace, LogType type)
        {
            var time = DateTime.Now.ToString("[HH:mm:ss]");
            var log = new Log { type = type, text = $"{time} {text}", stackTrace = stackTrace};
            this.logs.Add(log);
            this.OnLogAdd?.Invoke(log);

            switch(type)
            {
                case LogType.Warning:
                    if(this.Status is LogType.Log)
                    {
                        this.Status = LogType.Warning;
                        this.OnStatusChange?.Invoke(this.Status);
                    }
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if(this.Status is LogType.Log or LogType.Warning)
                    {
                        this.Status = LogType.Error;
                        this.OnStatusChange?.Invoke(this.Status);
                    }
                    break;
            }
        }
    }
}
