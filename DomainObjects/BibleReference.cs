using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DomainObjects
{
    public class BibleReference
    {
        public string Id { get; set; }
        public string BookShortName { get; set; }
        public LinkedList<BibleVersesReference> References { get; set; }
        public override string ToString()
        {
            return $"{BookShortName}{string.Join(';', References.Select(r => r.ToString()))}";
        }
    }

    public class BibleVersesReference
    {
        public int Chapter { get; set; }
        public LinkedList<FromToVerses> FromToVerses { get; set; }

        public override string ToString()
        {
            return $"{Chapter}:{string.Join(',', FromToVerses)}";
        }
    }

    public class FromToVerses
    {
        public int FromVerse { get; set; }
        public int? ToVerse { get; set; }
        public override string ToString()
        {
            string toVerseString = ToVerse == null ? String.Empty : $"-{ToVerse}";
            return $"{FromVerse}{toVerseString}";
        }
    }
}
