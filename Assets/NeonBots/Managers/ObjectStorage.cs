using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace NeonBots.Managers
{
    public class ObjectStorage : Manager
    {
        [Serializable]
        public class Item
        {
            public string key;

            public Object val;
        }

        public List<Item> storage = new();

        public void Add(string name, Object obj)
        {
            DontDestroyOnLoad(obj);
            this.storage.Add(new() { key = name, val = obj });
        }

        public Object Get(string name) => this.storage.FirstOrDefault(item => item.key == name)?.val;

        public bool TryGet(string name, out Object obj)
        {
            obj = this.storage.FirstOrDefault(item => item.key == name)?.val;
            return obj != default;
        }

        public bool TryGetItem(string name, out Item item)
        {
            item = this.storage.FirstOrDefault(item => item.key == name);
            return item != default;
        }

        public void Remove(string name)
        {
            if(!this.TryGetItem(name, out var item)) return;
            this.storage.Remove(item);
            Destroy(item.val);
        }
    }
}
