using BibleComponents;
using Microsoft.AspNetCore.Components;
using System;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public interface IInteractionModel
    {
        bool IsBottom { get; }
        public event Action OnClose;
        public void Close();
        Type ComponentType { get; }
    }
}
