using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.DomainObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DataSources
{
    public class VersesProvider
    {
        public Task VersesLoadTask { get; private set; }
        public BibleService Bible { get; set; }

        internal Parser Parser { get; set; }
        internal bool VersesLoaded { get => VersesLoadTask?.IsCompleted == true; }
        internal IEnumerable<BibleReference> BibleReferences { get => _references; }
        internal Dictionary<string, IEnumerable<BibleService.VersesView>> VersesViewsDictionary { get => _versesViewsDictionary; }

        private IEnumerable<BibleReference> _references;
        private Dictionary<string, IEnumerable<BibleService.VersesView>> _versesViewsDictionary;

        public async Task LoadVerses()
        {
            VersesLoadTask = LoadVersesInteranl();
            await VersesLoadTask;
        }
        private async Task LoadVersesInteranl()
        {
            _versesViewsDictionary = new Dictionary<string, IEnumerable<BibleService.VersesView>>();
            foreach (BibleReference reference in BibleReferences)
            {
                var key = reference.ToString();
                if (!_versesViewsDictionary.ContainsKey(key))
                    _versesViewsDictionary.Add(key, await Bible.GetVersesFromReference(reference));
            }
        }

        public VersesProvider(IEnumerable<BibleReference> references, BibleService Bible)
        {
            _references = references;
            this.Bible = Bible;
        }
    }
}
