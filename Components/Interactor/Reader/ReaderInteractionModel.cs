using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Pages.Lesson;
using Bible_Blazer_PWA.Services.Menu;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Reader
{
    public class ReaderInteractionModel : InteractionModelBase<ReaderInteractionModel>
    {
        public override bool IsSide => false;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(ReaderInteractionComponent);

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "",
                Action = () =>
                {
                    HomeInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = Icons.Material.Filled.Home
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "Reader",
                Action = () =>
                {
                    ReaderInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };
        }

        public event Action OnRefreshNeeded;
        protected void RefreshNeeded() => OnRefreshNeeded?.Invoke();

        #region Buttons
        public override IEnumerable<(IButtonStateHandler, IButtonVisibilityHandler)> GetButtons()
        {
            IconResolver iconReolver = new IconResolver();

            yield return (new FontSizeIncreaseHandler(iconReolver, RefreshNeeded), null);
            yield return (new FontSizeDecreaseHandler(iconReolver, RefreshNeeded), null);
        }
        #endregion
    }
}
