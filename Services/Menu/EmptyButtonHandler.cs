using System;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class EmptyButtonHandler : IButtonStateHandler
    {
        private readonly string key;
        private readonly string icon;
        public EmptyButtonHandler(string icon)
        {
            this.icon = icon;
            key = Guid.NewGuid().ToString();
        }
        public int StatesCount => 0;

        public string Key => key;

        public string GetIcon(int state)
        {
            return icon;
        }

        public void Handle(int state) { }
    }
}
