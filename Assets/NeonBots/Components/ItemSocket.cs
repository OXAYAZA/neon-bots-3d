using UnityEngine;

namespace NeonBots.Components
{
    public class ItemSocket : MonoBehaviour
    {
        public bool rotatable;

        public Item item;

        public void Use() => this.item.Use();
    }
}
