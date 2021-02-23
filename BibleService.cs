using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    public class BibleService
    {
        public class Verse
        {
            public int BookId { get; set; }
            public int Chapter { get; set; }
            public string Value { get; set; }
            public int Id { get; set; }
        }

        public class Book
        {
            public string Color { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
        }

        public class WeatherForecast
        {
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public string Summary { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }

        public string getVerseValue(string bookName, int chapter, int verse)
        {
            int bookId = _books.Where(b => (b.ShortName == bookName)).Select(b => b.Id).FirstOrDefault();
            return getVerseValue(bookId, chapter, verse);
        }

        public string getVerseValue(int bookId, int chapter, int verse)
        {
            string verseValue = _verses.Where(v => (v.Chapter == 3 && v.Id == 15 && v.BookId == bookId)).Select(v => v.Value).FirstOrDefault();
            return verseValue;
        }

        public string perfTest()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 10; i <= 100; i += 10)
            {
                for (int j = 1; j <= 2; j++)
                {
                    for (int k = 1; k <= 10; k++)
                    {
                        sb.Append(getVerseValue(i, j, k));
                    }
                }
            }
            return sb.ToString();
        }

        private WeatherForecast[] _forecasts;
        private Verse[] _verses;
        private Book[] _books;
        private bool _isLoaded = false;
        public bool IsLoaded { get { return _isLoaded; } }
        public void Init(WeatherForecast[] forecasts, Verse[] verses, Book[] books)
        {
            _forecasts = forecasts;
            _verses = verses;
            _books = books;
            _isLoaded = true;
        }
    }
}
