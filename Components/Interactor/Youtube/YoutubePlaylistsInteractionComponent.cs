using System;

namespace Bible_Blazer_PWA.Components.Interactor.Youtube
{
    public class YoutubePlaylistsInteractionModel : InteractionModelBase<YoutubePlaylistsInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(YoutubePlaylistsInteractionComponent);
    }
}
