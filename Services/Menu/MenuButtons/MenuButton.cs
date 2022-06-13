using System;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class MenuButton
    {
        internal MenuButton(IButtonStateHandler stateHandler)
        {
            OnClick += () => State = (State + 1) % stateHandler.StatesCount;
            OnClick += () => stateHandler.Handle(State);
            OnClick += () => Icon = stateHandler.GetIcon(State);
            Icon = stateHandler.GetIcon(State);
            State = 0;
        }
        private event Action OnClick;
        public void Click() => OnClick.Invoke();
        public string Icon { get; private protected set; }
        private protected int State { get; set; }
    }
}
