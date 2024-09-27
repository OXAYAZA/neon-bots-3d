using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NeonBots.Components
{
    public class ObjectLinks : MonoBehaviour
    {
        [Serializable]
        public class ObjectLink
        {
            public string name;

            public Object link;
        }

        public List<ObjectLink> links;

        public bool TryGetValue(string name, out Object value)
        {
            foreach(var link in this.links)
            {
                if(link.name == name)
                {
                    value = link.link;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
