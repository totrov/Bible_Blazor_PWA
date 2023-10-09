using Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter;
using Bible_Blazer_PWA.Components.Interactor.BibleViewer;
using static Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter.Interaction;
using static Bible_Blazer_PWA.Components.Interactor.BibleViewer.BibleViewerInteractionModel;

namespace Bible_Blazer_PWA.Components.Interactor.Transitions
{
    public class BibleReferenceWriterTransition : Transition<BibleReferencesWriterInteractionModel>
    {
        public override void ApplyTransition(BibleReferencesWriterInteractionModel source)
        {
            source.OnLinkClicked += (bookShortName, chapterNumber, verseNumber) =>
                BibleViewerInteractionModel.WithParameters<BookChapterVerse>
                    .Apply(new(bookShortName, chapterNumber, verseNumber), false);
        }
    }
}
