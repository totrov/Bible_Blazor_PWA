using System;
using System.Reflection.Metadata.Ecma335;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class MenuButton
    {
        private readonly IButtonVisibilityHandler visibilityHandler;

        internal MenuButton(IButtonStateHandler stateHandler, IButtonVisibilityHandler visibilityHandler)
        {
            if (stateHandler.StatesCount > 0)
            {
                OnClick += () => State = (State + 1) % stateHandler.StatesCount;
                OnClick += () => stateHandler.Handle(State);
                OnClick += () => Icon = stateHandler.GetIcon(State);
            }
            Icon = stateHandler.GetIcon(State);
            State = 0;
            this.visibilityHandler = visibilityHandler;
        }
        private event Action OnClick;
        public void Click() => OnClick.Invoke();
        public string Icon { get; private protected set; }
        private protected int State { get; set; }
        public bool Visible { get => visibilityHandler.IsVisible; }
        public bool IsClickable { get => OnClick is not null; }
    }
}
