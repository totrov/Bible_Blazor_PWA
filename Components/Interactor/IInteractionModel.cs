using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using Bible_Blazer_PWA.ViewModels;
using System;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public interface IInteractionModel
    {
        bool IsSide { get; }
        bool ShouldPersistInHistory { get; }

        public event Action OnClose;
        public void Close();
        Type ComponentType { get; }
        IInteractionModel Next { get; set; }
        IInteractionModel Previous { get; set; }
    }
    public abstract class InteractionModelBase<TSelf> : Interaction.InteractionModel<TSelf>, IInteractionModel
        where TSelf : InteractionModelBase<TSelf>
    {
        public abstract bool IsSide { get; }
        public abstract bool ShouldPersistInHistory { get; }

        public abstract Type ComponentType { get; }
        
        public IInteractionModel Next { get; set; }
        public IInteractionModel Previous { get; set; }
        public abstract event Action OnClose;
        public abstract void Close();
    }

    public abstract class Command<TSelf> : InteractionModelBase<TSelf>
        where TSelf: Command<TSelf>
    {
        public event Action OnCancel;
        public void Cancel() => OnCancel.Invoke();
        public event Action OnSuccess;
        public void HandleSuccess() => OnSuccess?.Invoke();
        public Command()
        {
            OnCancel += Interaction.GoToPrevious;
        }
    }
}
