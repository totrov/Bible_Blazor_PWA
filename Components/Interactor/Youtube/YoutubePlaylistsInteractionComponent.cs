using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.Setup;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Youtube
{
    public class YoutubePlaylistsInteractionModel : InteractionModelBase<YoutubePlaylistsInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(YoutubePlaylistsInteractionComponent);

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
                Text = "Плейлисты YouTube",
                Action = () =>
                {
                    YoutubePlaylistsInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };
        }
    }
}
