using System;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class SimpleActionButtonHandler : IButtonStateHandler
    {
        private readonly string icon;
        private readonly Action action;
        private readonly string key;

        public SimpleActionButtonHandler(string icon, Action action)
        {
            this.icon = icon;
            this.action = action;
            key = Guid.NewGuid().ToString();
        }
        public int StatesCount => 1;

        public string Key => key;

        public string GetIcon(int state) => icon;

        public void Handle(int state) => action();
    }
}
