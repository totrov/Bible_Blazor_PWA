using System.Collections.Generic;
using System.Threading.Tasks;
using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.TextHandlers;

namespace Bible_Blazer_PWA
{
    public class BibleService
    {
        private IBibleServiceFetchStrategy _dataProvider;
        public class Verse
        {
            public int BookId { get; set; }
            public int Chapter { get; set; }
            public string Value { get; set; }
            public int Id { get; set; }
        }

        public class VersesView
        {
            public string Badge { get; set; }
            public string RawText { get; set; }
            public (string BookShortName, int Verse) FirstVerseRef { get; set; }
        }

        public class Book
        {
            public string Color { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
        }

        public async Task<IEnumerable<VersesView>> GetVersesFromReference(BibleReference reference)
        {
            Task<int> bookIdTask = _dataProvider.GetBookIdByShortNameAsync(reference.BookShortName);
            LinkedList<VersesView> result = new LinkedList<VersesView>();
            string badge = "";
            foreach (BibleVersesReference versesReference in reference.References)
            {
                badge = $"{versesReference.Chapter}:";
                foreach (FromToVerses fromTo in versesReference.FromToVerses)
                {
                    Task<IEnumerable<Verse>> verseTask = _dataProvider.GetVersesAsync(await bookIdTask, versesReference.Chapter, fromTo.FromVerse, fromTo.ToVerse);
                    VersesView versesView = new VersesView();
                    string toVerse = fromTo.ToVerse == null ? "" : $"-{fromTo.ToVerse}";
                    versesView.Badge = $"{badge}{fromTo.FromVerse}{toVerse}";
                    versesView.RawText = _versesHandler.GetHtmlFromVerses(await verseTask, fromTo.ToVerse == null, _parametersModel.StartVersesOnANewLine == "True");
                    versesView.FirstVerseRef = (reference.BookShortName, fromTo.FromVerse);
                    result.AddLast(versesView);
                }
            }

            return result;
        }

        private bool _isLoaded = false;
        private VersesTextHandler _versesHandler;
        private ParametersModel _parametersModel;

        public bool IsLoaded { get { return _isLoaded; } }
        public void Init(DataBase.DatabaseJSFacade dataBase, ParametersModel parametersModel)
        {
            _dataProvider = new DataBaseBibleServiceFetchStrategy(dataBase);
            _isLoaded = true;
            _versesHandler = new VersesTextHandler(parametersModel.BibleRefVersesNumbersColor);
            _parametersModel = parametersModel;
        }
    }
}
