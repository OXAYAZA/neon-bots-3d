using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace NeonBots.Managers
{
    public class LocalConfig : Manager
    {
        public event Action<string> OnLocalValueChanged;

        private Dictionary<string, string> config;

        public void Init()
        {
            this.config = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                Resources.Load("local_config_defaults").ToString());
        }

        public T Get<T>(string name)
        {
            var val = this.config[name];

            if(typeof(T) == typeof(string)) return (T)(object)val;
            if(typeof(T) == typeof(bool)) return (T)(object)Convert.ToBoolean(val, CultureInfo.InvariantCulture);
            if(typeof(T) == typeof(int)) return (T)(object)Convert.ToInt32(val, CultureInfo.InvariantCulture);
            if(typeof(T) == typeof(float)) return (T)(object)Convert.ToSingle(val, CultureInfo.InvariantCulture);
            if(typeof(T) == typeof(double)) return (T)(object)Convert.ToDouble(val, CultureInfo.InvariantCulture);
            if(typeof(T) == typeof(long)) return (T)(object)Convert.ToInt64(val, CultureInfo.InvariantCulture);

            throw new($"[LocalConfig] Can't get value \"{name}\" or convert to {typeof(T)}");
        }

        public void Set<T>(string name, T value)
        {
            this.config[name] = value.ToString();
            this.OnLocalValueChanged?.Invoke(name);
        }
    }
}