using Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter;
using Bible_Blazer_PWA.Components.Interactor.BibleViewer;

namespace Bible_Blazer_PWA.Components.Interactor.Transitions
{
    public class BibleReferenceWriterTransition : Transition<BibleReferencesWriterInteractionModel>
    {
        public override void ApplyTransition(BibleReferencesWriterInteractionModel source)
        {
            source.OnLinkClicked += (bookShortName, chapterNumber, verseNumber) =>
                Interaction
                .ModelOfType<BibleViewerInteractionModel>
                .WithParameters<BibleViewerInteractionModel.Parameters>
                    .Apply(new(bookShortName, chapterNumber, verseNumber));
        }
    }
}
