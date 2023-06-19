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
        public string Title { get; set; }
        public event Action<int, int> OnResize;
        public void Resize(int topHeight, int bottomHeight) => OnResize?.Invoke(topHeight, bottomHeight);
        public event EventHandler OnUpdate;
        public void Update(object sender) => OnUpdate?.Invoke(sender, EventArgs.Empty);
        public Dictionary<string, MenuButton> Buttons { get; private set; }
        private Dictionary<string, MenuButton> _buttonCache;

        #region public interface
        public void SetLessonCenteredContainer(LessonCenteredContainer container)
            => _interactionCoordinator = new Interaction(container);
        #endregion

        public MenuService()
        {
            _buttonCache = new Dictionary<string, MenuButton>();
            Buttons = new Dictionary<string, MenuButton>();
        }
        public MenuButton AddMenuButton(IButtonStateHandler handler)
        {
            if (Buttons.ContainsKey(handler.Key))
                return Buttons[handler.Key];
            if (!_buttonCache.ContainsKey(handler.Key))
            {
                _buttonCache.Add(handler.Key, new(handler));
            }
            var button = _buttonCache[handler.Key];
            Buttons.Add(handler.Key, button);
            return button;
        }
        public void ClearMenuButtons()
        {
            _buttonCache.Clear();
            Buttons.Clear();
        }
    }
}
