using Bible_Blazer_PWA.Facades;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class BibleRegexHelper : IRegexHelper
    {
        private string spases = @"\s*";
        private string bookRegex = @"(?<book>Быт|Исх|Лев|Чис|Втор|Нав|Суд|Руфь|1Цар|2Цар|3Цар|4Цар|1Пар|2Пар|Ездр|Неем|Есф|Иов|Пс|Прит|Еккл|Песн|Ис|Иер|Плач|Иез|Дан|Ос|Иоил|Ам|Авд|Ион|Мих|Наум|Авв|Соф|Агг|Зах|Мал|Мат|Мар|Лук|Ин|Деян|Иак|1Пет|2Пет|1Ин|2Ин|3Ин|Иуд|Рим|1Кор|2Кор|Гал|Еф|Флп|Кол|1Фес|2Фес|1Тим|2Тим|Тит|Флм|Евр|Откр)\.?";
        private string chapterRegex = @"(?<chapter>\d+)\s*:";
        private string fromRegex = @"(?<from>\d+)";
        private string toRegex = @"(?:\s*-\s*(?<to>\d+))?";

        public string GetBibleReferencesPattern()
        {
            string wholeRegex = string.Join(spases,
                bookRegex,
                @"(?:;?(?<ref>",
                    chapterRegex,
                    @"(?:,?(?<fromTo>",
                        fromRegex, toRegex,
                    @"))+",
                @"))+"
            );

            return wholeRegex;
        }

        public string GetBracketsHandlerPattern()
        {
            string bracketsHandleRegex = string.Join(spases,
                bookRegex,
                @"[\s\d:;,-]+",
                @"[(](?<bracketsContent>[\s\d:,;-]+)[)]"
            );

            return bracketsHandleRegex;
        }

        public string GetBibleVerseReferencesPattern()
        {
            string refRegex = string.Join(spases,
                chapterRegex,
                @"(?:,?(?<fromTo>",
                    fromRegex, toRegex,
                @"))+"
            );

            return refRegex;
        }

        public string GetFromToVersesPattern()
        {
            string fromToVersesRegex = string.Join(spases,
                @"(?:,?(?:",
                        fromRegex, toRegex,
                @"))+"
            );

            return fromToVersesRegex;
        }

        public string GetLessonsPattern()
        {
            List<string> negativeLookaheads = GetNegativeLookaheadsForLessonHeaders();
            StringBuilder sb = new();
            sb.Append("(?:\r|^)[\\s]*([0-9]+[.]?[0-9]*[.]");
            foreach (var lookahead in negativeLookaheads)
            {
                sb.Append("(?!");
                sb.Append(lookahead);
                sb.Append(")");
            }
            sb.Append(".*?)\r");
            return sb.ToString();
        }

        public string GetSublessonHeaderPattern(bool namedHeaderGroup)
        {
            var headerNamePart = namedHeaderGroup ? "(?<header>.*?)" : ".*?";
            return "(?:^|(?:<br>))[0-9]{1,2}[.]" + headerNamePart + "(?=1[)])";
        }

        public string GetSublessonsPattern()
        {
            var lookbehind = GetSublessonHeaderPattern(true);
            var lookahead = GetSublessonHeaderPattern(false);
            return $"(?<={lookbehind}).*?(?=(?:{lookahead}|$))";
        }

        Dictionary<string, Dictionary<string, string>> replacements = null;
        List<string> negativeLookaheadsForLessonHeaders = null;
        HttpFacade HttpFacade;

        public BibleRegexHelper(HttpClient Http)
        {
            HttpFacade = new HttpFacade(Http);
        }

        public List<string> GetNegativeLookaheadsForLessonHeaders()
        {
            return negativeLookaheadsForLessonHeaders;
        }

        public async Task Init()
        {
            await Task.WhenAll(InitReplacements(), InitOther());
        }
        public async Task InitReplacements()
        {
            replacements = await HttpFacade.GetRepacementsFromJsonAsync();
        }
        public async Task InitOther()
        {
            negativeLookaheadsForLessonHeaders = await HttpFacade.GetNegativeLookaheads();
        }
        public Dictionary<string, Dictionary<string, string>> GetReplacements()
        {
            return replacements;
        }
    }
}
