using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using Bible_Blazer_PWA.ViewModels;
using System;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public interface IInteractionModel
    {
        bool IsSide { get; }

        public event Action OnClose;
        public void Close();

        public event Action<int> OnResize;
        public void Resize(int size);

        public void ComponentInitialized();
        public event Action OnComponentInitialized;

        Type ComponentType { get; }
        IInteractionModel Next { get; set; }
        IInteractionModel Previous { get; set; }
    }
    public interface IInteractionModelParameters<TInteractionModel> where TInteractionModel:IInteractionModel, new()
    {
        void ApplyParametersToModel(TInteractionModel model);
    }

    public abstract class InteractionModelBase : IInteractionModel
    {
        public abstract bool IsSide { get; }
        public abstract Type ComponentType { get; }
        
        public IInteractionModel Next { get; set; }
        public IInteractionModel Previous { get; set; }
        
        public abstract event Action OnClose;
        public abstract void Close();

        public event Action<int> OnResize;
        public void Resize(int size) => OnResize?.Invoke(size);

        public event Action OnComponentInitialized;
        public void ComponentInitialized() => OnComponentInitialized?.Invoke();
    }

    public abstract class Command : InteractionModelBase
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
