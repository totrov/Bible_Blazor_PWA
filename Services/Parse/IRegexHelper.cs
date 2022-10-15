using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public interface IRegexHelper
    {
        string GetBibleReferencesPattern();
        string GetBibleVerseReferencesPattern();
        string GetBracketsHandlerPattern();
        string GetFromToVersesPattern();
        string GetLessonsPattern();
        List<string> GetNegativeLookaheadsForLessonHeaders();
        Dictionary<string, Dictionary<string, string>> GetReplacements();
        string GetSublessonHeaderPattern(bool namedHeaderGroup);
        string GetSublessonsPattern();
        Task Init();
        Task InitOther();
        Task InitReplacements();
    }
}