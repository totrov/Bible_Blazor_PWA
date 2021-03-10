using System;

namespace Bible_Blazer_PWA.BibleReferenceParse
{
    internal class ToRegex
    {
        internal string GetPattern()
        {
            return @"(?:\s*-\s*(?<to>\d+))?";
        }
    }
}