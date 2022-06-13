using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class MenuService
    {
        public string Title { get; set; }
        public event EventHandler OnUpdate;
        public void Update(object sender) => OnUpdate?.Invoke(sender, EventArgs.Empty);
        public Dictionary<string, MenuButton> Buttons { get; private set; }
        private Dictionary<string, MenuButton> _buttonCache;
        public MenuService()
        {
            _buttonCache = new Dictionary<string, MenuButton>();
            Buttons = new Dictionary<string, MenuButton>();
        }
        public void AddMenuButton(IButtonStateHandler handler)
        {
            if (Buttons.ContainsKey(handler.Key))
                return;
            if (!_buttonCache.ContainsKey(handler.Key))
            {
                _buttonCache.Add(handler.Key, new(handler));
            }
            Buttons.Add(handler.Key, _buttonCache[handler.Key]);
        }
        public void ClearMenuButtons()
        {
            _buttonCache.Clear();
            Buttons.Clear();
        }
    }
}
