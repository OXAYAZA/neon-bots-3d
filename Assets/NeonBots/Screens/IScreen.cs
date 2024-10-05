using System.Collections.Generic;

namespace NeonBots.Screens
{
    public interface IScreen
    {
        public void Switch(bool state = true);

        public void Open();

        public void Close();

        public void GoTo();

        public void Back();

        // TODO: IsPopup should be renamed.
        public bool IsPopup();

        public bool IsBlockBack();

        public void OnBeforeDisable();

        public Dictionary<string, object> GetState();

        public void SetState(Dictionary<string, object> data);
    }
}
