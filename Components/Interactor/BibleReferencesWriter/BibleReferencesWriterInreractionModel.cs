using Bible_Blazer_PWA.Components.Interactor.Bible;
using Bible_Blazer_PWA.Components.Interactor.Home;
using BibleComponents;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bible_Blazer_PWA.Components.Interactor.Bible.BibleChaptersInteractionModel;
using static Bible_Blazer_PWA.Components.Interactor.Bible.BibleInteractionModel;

namespace Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter
{
    public partial class Interaction
    {
        public class BibleReferencesWriterInteractionModel : InteractionModelBase<BibleReferencesWriterInteractionModel>
        {
            public override bool IsSide => true;
            public override bool ShouldPersistInHistory => true;
            public override Type ComponentType => typeof(BibleReferencesWriterInteractionComponent);
            public override string Background => "beige";
            public event Action<string, int, int> OnLinkClicked;
            public void LinkClicked(string BookShortName, int ChapterNumber, int Verse)
                => OnLinkClicked?.Invoke(BookShortName, ChapterNumber, Verse);
            public void MouseLeave() { if (!Overflowed) Close(); }

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

                string bookShortName = Mediator.VersesViewsDictionary.First().Value.First().FirstVerseRef.BookShortName;
                yield return new BreadcrumbsFacade.BreadcrumbRecord
                {
                    Text = bookShortName,
                    Action = () =>
                    {
                        BibleChaptersInteractionModel.WithParameters<BibleBookShortName>
                            .ApplyToCurrentPanel(new (bookShortName), this);
                    },
                    Icon = null
                };
            }

            public LessonElementMediator Mediator { get; set; }
            public int ReferenceNumber { get; set; }
            public bool Overflowed { get; set; } = false;


            public class MediatorReferenceNumber : Parameters
            {
                public LessonElementMediator LessonElementMediator { get; set; }
                public int ReferenceNumber { get; private set; }
                public MediatorReferenceNumber(LessonElementMediator lessonElementMediator, int referenceNumber)
                {
                    LessonElementMediator = lessonElementMediator;
                    ReferenceNumber = referenceNumber;
                }
                public override void ApplyParametersToModel(BibleReferencesWriterInteractionModel model)
                {
                    model.Mediator = LessonElementMediator;
                    model.ReferenceNumber = ReferenceNumber;
                }
            }
        }
    }
}
