using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public static class BibleRegexHelper
    {
        private static string spases = @"\s*";
        private static string bookRegex = @"(?<book>Быт|Исх|Лев|Чис|Втор|Нав|Суд|Руфь|Ездр|Неем|Есф|Иов|Пс|Прит|Еккл|Песн|Ис|Иер|Плач|Иез|Дан|Ос|Иоил|Ам|Авд|Ион|Мих|Наум|Авв|Соф|Агг|Зах|Мал|Мат|Мар|Лук|Ин|Деян|Иак|Иуд|Рим|Гал|Еф|Флп|Кол|Тит|Флм|Евр|Откр)\.?";
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
    }
}
