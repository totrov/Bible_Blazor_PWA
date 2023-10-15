using DocumentFormat.OpenXml.Bibliography;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.BibleViewer
{
    public class BibleViewerInteractionModel : InteractionModelBase<BibleViewerInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => true;
        public override Type ComponentType => typeof(BibleViewerInteractionComponent);
        public string BookShortName { get; set; }
        public int VerseNumber { get; set; }
        public int ChapterNumber { get; set; }        

        public class BookChapterVerse : Parameters
        {
            public BookChapterVerse(string bookShortName, int chapterNumber, int verseNumber)
            {
                BookShortName = bookShortName;
                ChapterNumber = chapterNumber;
                VerseNumber = verseNumber;
            }

            public string BookShortName { get; }
            public int VerseNumber { get; }
            public int ChapterNumber { get; set; }

            public override void ApplyParametersToModel(BibleViewerInteractionModel model)
            {
                model.BookShortName = BookShortName;
                model.VerseNumber = VerseNumber;
                model.ChapterNumber = ChapterNumber;
            }
        }
    }
}
