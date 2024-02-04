using Bible_Blazer_PWA.Components.Interactor.Home;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Youtube
{
    public class YoutubeVideoInteractionModel : InteractionModelBase<YoutubeVideoInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(YoutubeVideoInteractionComponent);
        public string YoutubeSrc { get; set; }
        public string Name { get; set; }

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            if (IsMainContent)
            {
                return new[]{
                    new BreadcrumbsFacade.BreadcrumbRecord
                    {
                        Text = "",
                        Action = () =>
                        {
                            HomeInteractionModel.ApplyToCurrentPanel(this);
                        },
                        Icon = Icons.Material.Filled.Home
                    },
                    new BreadcrumbsFacade.BreadcrumbRecord
                    {
                        Text = "Плейлисты YouTube",
                        Action = () =>
                        {
                            YoutubePlaylistsInteractionModel.ApplyToCurrentPanel(this);
                        },
                        Icon = null
                    },

                    new BreadcrumbsFacade.BreadcrumbRecord
                    {
                        Text = Name,
                        Action = () =>
                        {
                            YoutubePlaylistsInteractionModel.ApplyToCurrentPanel(this);
                        },
                        Icon = null
                    }
                };
            }
            else return null;
        }

        public class YoutubeSourceAndName : Parameters
        {
            public string YoutubeSrc { get; set; }
            public string Name { get; set; }
            public YoutubeSourceAndName(string youtubeSrc, string name)
            {
                YoutubeSrc = youtubeSrc;
                Name = name;
            }
            public override void ApplyParametersToModel(YoutubeVideoInteractionModel model)
            {
                model.YoutubeSrc = YoutubeSrc;
                model.Name = Name;
            }
        }
    }
}
