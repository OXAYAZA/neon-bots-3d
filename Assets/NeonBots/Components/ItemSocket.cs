using UnityEngine;

namespace NeonBots.Components
{
    public class ItemSocket : MonoBehaviour
    {
        public Item item;

        public void Use() => this.item.Use();
    }
}
