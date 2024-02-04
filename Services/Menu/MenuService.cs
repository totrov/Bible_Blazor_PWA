using Bible_Blazer_PWA.Components.Interactor;
using BibleComponents;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class MenuService
    {
        private Interaction _interactionCoordinator;
        public Interaction InteractionCoordinator => _interactionCoordinator;
        public event Action<int, int> OnResize;
        public void Resize(int topHeight, int bottomHeight) => OnResize?.Invoke(topHeight, bottomHeight);
        public event EventHandler OnUpdate;
        public void Update(object sender) => OnUpdate?.Invoke(sender, EventArgs.Empty);
        public Dictionary<string, MenuButton> Buttons { get; private set; }
        private Dictionary<string, MenuButton> _buttonCache;
        private DefaultVisibilityHandler defaultVisibilityHandler;

        public MenuService()
        {
            _buttonCache = new Dictionary<string, MenuButton>();
            Buttons = new Dictionary<string, MenuButton>();
            defaultVisibilityHandler = new DefaultVisibilityHandler();
        }
        public MenuButton AddMenuButton(IButtonStateHandler stateHandler, IButtonVisibilityHandler visibilityHandler)
        {
            if (Buttons.ContainsKey(stateHandler.Key))
                return Buttons[stateHandler.Key];
            if (!_buttonCache.ContainsKey(stateHandler.Key))
            {
                _buttonCache.Add(stateHandler.Key, new(stateHandler, visibilityHandler));
            }
            var button = _buttonCache[stateHandler.Key];
            Buttons.Add(stateHandler.Key, button);
            return button;
        }

        public MenuButton AddMenuButton(IButtonStateHandler stateHandler)
        {
            return AddMenuButton(stateHandler, defaultVisibilityHandler);
        }

        public void ClearMenuButtons()
        {
            _buttonCache.Clear();
            Buttons.Clear();
        }
    }
}
