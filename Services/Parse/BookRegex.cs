using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class BookRegex
    {
        public string GetPattern()
        {
            return
                @"(?<book>Быт|Исх|Лев|Чис|Втор|Нав|Суд|Руфь|Ездр|Неем|Есф|Иов|Пс|Прит|Еккл|Песн|Ис|Иер|Плач|Иез|Дан|Ос|Иоил|Ам|Авд|Ион|Мих|Наум|Авв|Соф|Агг|Зах|Мал|Мат|Мар|Лук|Ин|Деян|Иак|Иуд|Рим|Гал|Еф|Флп|Кол|Тит|Флм|Евр|Откр)\.?";
        }

        internal BibleReference Interpret(Match match)
        {
            BibleReference bibleReference = new BibleReference();
            bibleReference.BookShortName = match.Groups.Where(g => g.Name == "book").First().Value;
            ChapterRegex chapterRegex = new ChapterRegex();
            int curIndex = 0;
            var chapterCaptures = match.Groups.Where(g => g.Name == "chapter").First().Captures.ToArray();
            var fromToCaptures = match.Groups.Where(g => g.Name == "fromTo").First().Captures.ToArray();
            var fromCaptures = match.Groups.Where(g => g.Name == "from").First().Captures.ToArray();
            foreach (Capture capture in match.Groups.Where(g => g.Name == "ref").First().Captures)
            {
                BibleVersesReference versesReference = new BibleVersesReference();
                versesReference.Chapter = int.Parse(chapterCaptures[curIndex].Value);
                LinkedList<FromToVerses> fromToVerses = new LinkedList<FromToVerses>();
                foreach(Capture fromToCapture in match.Groups.Where(g => g.Name == "ref").First().Captures)
                {
                    FromToVerses fromTo = new FromToVerses();
                    fromTo.FromVerse = 
                    versesReference.FromToVerses = new FromToVerses { FromVerse = int.Parse(chapterCaptures[curIndex].Value), ToVerse = ParseToVerse() };
                }
                versesReference.FromToVerses = fromToVerses;
                bibleReference.References.AddLast(versesReference);
            }
            return bibleReference;
        }

        private int? ParseToVerse()
        {
            throw new NotImplementedException();
        }

        protected string GetGroupName()
        {
            return "book";
        }
    }
}
