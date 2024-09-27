using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Managers
{
    public class ObjectStorage : Manager
    {
        private Dictionary<string, Object> storage = new();

        public void Add(string name, Object obj)
        {
            DontDestroyOnLoad(obj);
            this.storage.Add(name, obj);
        }

        public Object Get(string name) => this.storage[name];

        public bool TryGet(string name, out Object obj) => this.storage.TryGetValue(name, out obj);

        public void Remove(string name)
        {
            if(!this.storage.TryGetValue(name, out var obj)) return;
            Destroy(obj);
        }
    }
}
