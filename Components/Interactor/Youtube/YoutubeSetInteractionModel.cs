using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.DataBase.DTO;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Youtube
{
    public class YoutubeSetInteractionModel : InteractionModelBase<YoutubeSetInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(YoutubeSetInteractionComponent);

        public string Name { get; set; }
        public IEnumerable<YoutubeLinkDTO> Links { get; set; }

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
                Text = "YouTube",
                Action = () =>
                {
                    YoutubePlaylistsInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = Name,
                Action = () =>
                {
                    YoutubeSetInteractionModel.WithParameters<NameAndLinks>.ApplyToCurrentPanel(new(Name, Links), this);
                },
                Icon = null
            };
        }

        public class NameAndLinks : Parameters
        {
            public string Name { get; set; }

            public NameAndLinks(string name, IEnumerable<YoutubeLinkDTO> links)
            {
                Name = name;
                Links = links;
            }

            public IEnumerable<YoutubeLinkDTO> Links { get; set; }

            public override void ApplyParametersToModel(YoutubeSetInteractionModel model)
            {
                model.Links = Links;
                model.Name = Name;
            }
        }
    }
}
