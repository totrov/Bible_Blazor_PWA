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

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            return null;
        }

        public class YoutubeSource : Parameters
        {
            public string YoutubeSrc { get; set; }
            public YoutubeSource(string youtubeSrc)
            {
                YoutubeSrc = youtubeSrc;
            }
            public override void ApplyParametersToModel(YoutubeVideoInteractionModel model)
            {
                model.YoutubeSrc = YoutubeSrc;
            }
        }
    }
}
