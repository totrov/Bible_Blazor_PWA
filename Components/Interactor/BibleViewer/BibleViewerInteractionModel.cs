using Bible_Blazer_PWA.Components.Interactor.Bible;
using Bible_Blazer_PWA.Components.Interactor.Home;
using DocumentFormat.OpenXml.Bibliography;
using MudBlazor;
using System;
using System.Collections.Generic;
using static Bible_Blazer_PWA.Components.Interactor.Bible.BibleChaptersInteractionModel;
using static Bible_Blazer_PWA.Components.Interactor.Bible.BibleInteractionModel;

namespace Bible_Blazer_PWA.Components.Interactor.BibleViewer
{
    public class BibleViewerInteractionModel : InteractionModelBase<BibleViewerInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => true;
        public override Type ComponentType => typeof(BibleViewerInteractionComponent);
        public override string Background => "beige";
        public string BookShortName { get; set; }
        public int VerseNumber { get; set; }
        public int ChapterNumber { get; set; }

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
                Text = "Библия",
                Action = () =>
                {
                    BibleInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = Icons.Material.Filled.Book
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = BookShortName,
                Action = () =>
                {
                    BibleChaptersInteractionModel.WithParameters<BibleBookShortName>
                        .ApplyToCurrentPanel(new (BookShortName), this);
                },
                Icon = null
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = $"{ChapterNumber} глава",
                Action = () =>
                {
                    BibleViewerInteractionModel.WithParameters<BookChapterVerse>.ApplyToCurrentPanel(new BookChapterVerse(
                        BookShortName = BookShortName,
                        ChapterNumber = ChapterNumber,
                        VerseNumber = VerseNumber), this);
                },
                Icon = null
            };
        }

        public class BookChapterVerse : Parameters
        {
            public BookChapterVerse(string bookShortName, int chapterNumber, int verseNumber)
            {
                BookShortName = bookShortName;
                ChapterNumber = chapterNumber;
                VerseNumber = verseNumber;
            }

            public string BookShortName { get; set; }
            public int VerseNumber { get; set; }
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
