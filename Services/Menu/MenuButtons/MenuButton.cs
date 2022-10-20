using System;
using System.Reflection.Metadata.Ecma335;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class MenuButton
    {
        internal MenuButton(IButtonStateHandler stateHandler)
        {
            if (stateHandler.StatesCount > 0)
            {
                OnClick += () => State = (State + 1) % stateHandler.StatesCount;
                OnClick += () => stateHandler.Handle(State);
                OnClick += () => Icon = stateHandler.GetIcon(State);
            }
            Icon = stateHandler.GetIcon(State);
            State = 0;
        }
        private event Action OnClick;
        public void Click() => OnClick.Invoke();
        public string Icon { get; private protected set; }
        private protected int State { get; set; }
        public bool Visible { get; set; }
        public bool IsClickable { get => OnClick is not null; }
    }
}
