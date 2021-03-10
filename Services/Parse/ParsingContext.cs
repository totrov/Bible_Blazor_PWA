using Bible_Blazer_PWA.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class ParsingContext
    {
        private string _stringToParse;
        private LinkedList<BibleReference> _result;
        public ParsingContext(string stringToParse)
        {
            _stringToParse = stringToParse;
            _result = new LinkedList<BibleReference>();
        }
    }
}
