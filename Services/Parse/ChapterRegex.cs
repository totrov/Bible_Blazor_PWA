using System;

namespace Bible_Blazer_PWA.BibleReferenceParse
{
    internal class ChapterRegex
    {
        internal string GetPattern()
        {
            return @"(?<chapter>\d+)\s*:";
        }

        internal string GetGroupName()
        {
            return @"ref";
        }
    }
}