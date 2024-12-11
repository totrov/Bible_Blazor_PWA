using Bible_Blazer_PWA.Components.Interactor.Bible;
using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.DataSources;
using Bible_Blazer_PWA.Static;

using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bible_Blazer_PWA.Components.Interactor.Bible.BibleChaptersInteractionModel;


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
            public event Action<string, int> OnLinkClicked;
            public void LinkClicked(string BookShortName, int ChapterNumber)
                => OnLinkClicked?.Invoke(BookShortName, ChapterNumber);
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
                    Icon = Constants.BibleIcon
                };

                string bookShortName = VersesProvider.BibleReferences.ElementAt(ReferenceNumber).BookShortName;
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

            public VersesProvider VersesProvider { get; set; }
            public int ReferenceNumber { get; set; }
            public bool Overflowed { get; set; } = false;


            public class VersesProviderReferenceNumber : Parameters
            {
                public VersesProvider VersesProvider { get; set; }
                public int ReferenceNumber { get; private set; }
                public VersesProviderReferenceNumber(VersesProvider versesProvider, int referenceNumber)
                {
                    VersesProvider = versesProvider;
                    ReferenceNumber = referenceNumber;
                }
                public override void ApplyParametersToModel(BibleReferencesWriterInteractionModel model)
                {
                    model.VersesProvider = VersesProvider;
                    model.ReferenceNumber = ReferenceNumber;
                }
            }
        }
    }
}
