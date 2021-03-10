using System;

namespace Bible_Blazer_PWA.BibleReferenceParse
{
    internal class FromRegex
    {
        public FromRegex()
        {
        }

        internal string GetPattern()
        {
            return @"(?<from>\d+)";
        }
    }
}