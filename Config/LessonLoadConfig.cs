using System.Collections.Generic;

namespace Bible_Blazer_PWA.Config
{
    public class LessonLoadConfig
    {
        private static Dictionary<string, string> lessonNameUrl = new()
        {
            {"Бытие","" },
            {"Исход - Соломон","" },
            {"Пророки","" },
            {"Евангелия","" },
            {"Деяния - Откровение","" },
            {"Основы веры","" }
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
