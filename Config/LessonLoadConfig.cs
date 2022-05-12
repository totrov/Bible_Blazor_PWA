using System.Collections.Generic;

namespace Bible_Blazer_PWA.Config
{
    public class LessonLoadConfig
    {
        private static Dictionary<string, string> lessonNameUrl = new()
        {
            {"Бытие", "https://bibleblazorpwastorage.blob.core.windows.net/lesson-units/Бытие.docx" },
            {"Исход - Соломон", "https://bibleblazorpwastorage.blob.core.windows.net/lesson-units/Исход - Соломон.docx" },
            {"Пророки", "https://bibleblazorpwastorage.blob.core.windows.net/lesson-units/Пророки..docx" },
            {"Евангелия", "https://bibleblazorpwastorage.blob.core.windows.net/lesson-units/Евангелия..docx" },
            {"Деяния - Откровение", "https://bibleblazorpwastorage.blob.core.windows.net/lesson-units/Деяния - Откровение..docx" },
            {"Основы веры", "https://bibleblazorpwastorage.blob.core.windows.net/lesson-units/Основы веры..docx" }
        };

        public static IEnumerable<string> GetLessonNames()
        {
            return lessonNameUrl.Keys;
        }

        public static string GetUrlByLessonName(string lessonName)
        {
            return lessonNameUrl[lessonName];
        }
    }
}
