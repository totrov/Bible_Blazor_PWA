using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public static class BibleRegexHelper
    {
        private static string spases = @"\s*";
        private static string bookRegex = @"(?<book>Быт|Исх|Лев|Чис|Втор|Нав|Суд|Руфь|1Цар|2Цар|3Цар|4Цар|1Пар|2Пар|Ездр|Неем|Есф|Иов|Пс|Прит|Еккл|Песн|Ис|Иер|Плач|Иез|Дан|Ос|Иоил|Ам|Авд|Ион|Мих|Наум|Авв|Соф|Агг|Зах|Мал|Мат|Мар|Лук|Ин|Деян|Иак|1Пет|2Пет|1Ин|2Ин|3Ин|Иуд|Рим|1Кор|2Кор|Гал|Еф|Флп|Кол|1Фес|2Фес|1Тим|2Тим|Тит|Флм|Евр|Откр)\.?";
        private static string chapterRegex = @"(?<chapter>\d+)\s*:";
        private static string fromRegex = @"(?<from>\d+)";
        private static string toRegex = @"(?:\s*-\s*(?<to>\d+))?";

        public static string GetBibleReferencesPattern()
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

        public static string GetBracketsHandlerPattern()
        {
            string bracketsHandleRegex = string.Join(spases,
                bookRegex,
                @"[\s\d:;,-]+",
                @"[(](?<bracketsContent>[\s\d:,;-]+)[)]"
            );

            return bracketsHandleRegex;
        }

        internal static string GetBibleVerseReferencesPattern()
        {
            string refRegex = string.Join(spases,
                chapterRegex,
                @"(?:,?(?<fromTo>",
                    fromRegex, toRegex,
                @"))+"
            );

            return refRegex;
        }

        internal static string GetFromToVersesPattern()
        {
            string fromToVersesRegex = string.Join(spases,
                @"(?:,?(?:",
                        fromRegex, toRegex,
                @"))+"
            );

            return fromToVersesRegex;
        }

        internal static string GetLessonsPattern()
        {
            List<string> negativeLookaheads = new List<string>()
            {
                "[0-9]",
                "Тело – плоть",
                "Дух – сердце",
                "Душа – разум",
                "Сторона",
                "Дн.",
                "Обзор книги Деяния",
                "Божий суверенитет",
                "Обзор послания к Римлянам",
                "Брак ",
                "Церковная",
                "Финансовое служение ",
                "Дары ",
                "Понимание любви Божьей",
                "Иные ",
                "Крещение С.Д."
            };
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

        internal static string GetSublessonHeaderPattern(bool namedHeaderGroup)
        {
            var headerNamePart = namedHeaderGroup ? "(?<header>.*?)" : ".*?";
            return "(?:^|(?:<br>))[0-9]{1,2}[.]"+ headerNamePart + "(?=1[)])";
        }

        internal static string GetSublessonsPattern()
        {
            //var lookaround = "(?:^|(?:<br>))[0-9]{1,2}[.](?=1[)])";
            var lookbehind = GetSublessonHeaderPattern(true);
            var lookahead = GetSublessonHeaderPattern(false);
            return $"(?<={lookbehind}).*?(?=(?:{lookahead}|$))";
        }
    }
}
