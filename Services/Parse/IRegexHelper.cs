using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public interface IRegexHelper
    {
        string GetBibleReferencesPattern();
        string GetBibleVerseReferencesPattern();
        string GetBibleReferencesPattern_ChapterOnly();
        string GetBracketsHandlerPattern();
        string GetFromToVersesPattern();
        string GetLessonsPattern();
        List<string> GetNegativeLookaheadsForLessonHeaders();
        Dictionary<string, Dictionary<string, string>> GetReplacements();
        Dictionary<string, Dictionary<string, string>> GetContinualReplacements();
        string GetSublessonHeaderPattern(bool namedHeaderGroup);
        string GetSublessonsPattern();
        Task Init();
        Task InitOther();
        Task InitReplacements();
    }
}