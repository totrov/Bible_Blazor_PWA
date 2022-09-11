using System.Collections.Generic;

namespace Bible_Blazer_PWA.Config
{
    public class LessonLoadConfig
    {
        private static Dictionary<string, string> lessonNameUrl = new()
        {
            {"Бытие", "/Assets/lessons/Бытие.docx" },
            {"Исход - Соломон", "/Assets/lessons/Исход - Соломон.docx" },
            {"Пророки", "/Assets/lessons/Пророки..docx" },
            {"Евангелия", "/Assets/lessons/Евангелия..docx" },
            {"Деяния - Откровение", "/Assets/lessons/Деяния - Откровение..docx" },
            {"Основы веры", "/Assets/lessons/Основы веры..docx" }
        };

        public static IEnumerable<string> GetLessonNames()
        {
            return lessonNameUrl.Keys;
        }

        public static string GetUrlByLessonName(string lessonName)
        {
            return lessonNameUrl[lessonName];
        }

        public static string GetManifestUrl()
        {
            return "https://covenant-of-christ.onrender.com/Assets/lessons/manifest.json";
        }
    }
}
