using DocumentFormat.OpenXml.Bibliography;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.BibleViewer
{
    public class BibleViewerInteractionModel : InteractionModelBase
    {
        public override bool IsBottom => true;
        public override Type ComponentType => typeof(BibleViewerInteractionComponent);
        public override event Action OnClose;
        public override void Close() => OnClose?.Invoke();
        public string BookShortName { get; set; }
        public int VerseNumber { get; set; }
        public int ChapterNumber { get; set; }        

        public class Parameters : IInteractionModelParameters<BibleViewerInteractionModel>
        {
            public Parameters(string bookShortName, int chapterNumber, int verseNumber)
            {
                BookShortName = bookShortName;
                ChapterNumber = chapterNumber;
                VerseNumber = verseNumber;
            }

            public string BookShortName { get; }
            public int VerseNumber { get; }
            public int ChapterNumber { get; set; }

            public void ApplyParametersToModel(BibleViewerInteractionModel model)
            {
                model.BookShortName = BookShortName;
                model.VerseNumber = VerseNumber;
                model.ChapterNumber = ChapterNumber;
            }
        }
    }
}
