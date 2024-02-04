using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using Bible_Blazer_PWA.Services.Menu;
using Bible_Blazer_PWA.ViewModels;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public interface IInteractionModel
    {
        bool IsSide { get; }
        bool ShouldPersistInHistory { get; }
        bool IsMainContent { get; set; }
        public event Action OnClose;
        public void Close();
        Type ComponentType { get; }
        public string Background { get; }
        IInteractionModel Next { get; set; }
        IInteractionModel Previous { get; set; }
        IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs();
        IEnumerable<(IButtonStateHandler, IButtonVisibilityHandler)> GetButtons();
    }
    public abstract class InteractionModelBase<TSelf> : Interaction.InteractionModel<TSelf>, IInteractionModel
        where TSelf : InteractionModelBase<TSelf>
    {
        public abstract bool IsSide { get; }
        public abstract bool ShouldPersistInHistory { get; }

        public abstract Type ComponentType { get; }
        
        public IInteractionModel Next { get; set; }
        public IInteractionModel Previous { get; set; }
        public bool IsMainContent { get; set; }

        public virtual string Background => "white";

        public event Action OnClose;
        public void Close() => OnClose?.Invoke();

        public abstract IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs();

        public virtual IEnumerable<(IButtonStateHandler, IButtonVisibilityHandler)> GetButtons()
        {
            return new (IButtonStateHandler, IButtonVisibilityHandler)[0];
        }
    }
    public abstract class CenteredInteractionModelBase<TSelf> : InteractionModelBase<TSelf>
        where TSelf : CenteredInteractionModelBase<TSelf>
    {
        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            return null;
        }
        public override bool IsSide { get => false; }
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
