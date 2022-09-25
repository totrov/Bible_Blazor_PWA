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

        private static Dictionary<string, string> lessonNameUrlOnline = new()
        {
            {"Бытие", "https://covenant-of-christ.onrender.com/Assets/online/lessons/Бытие.docx" },
            {"Исход - Соломон", "https://covenant-of-christ.onrender.com/Assets/online/lessons/Исход - Соломон.docx" },
            {"Пророки", "https://covenant-of-christ.onrender.com/Assets/online/lessons/Пророки..docx" },
            {"Евангелия", "https://covenant-of-christ.onrender.com/Assets/online/lessons/Евангелия..docx" },
            {"Деяния - Откровение", "https://covenant-of-christ.onrender.com/Assets/online/lessons/Деяния - Откровение..docx" },
            {"Основы веры", "https://covenant-of-christ.onrender.com/Assets/online/lessons/Основы веры..docx" }
        };

        public static IEnumerable<string> GetLessonNames()
        {
            return lessonNameUrl.Keys;
        }

        public static string GetUrlByLessonName(string lessonName, bool online = false)
        {
            return online
                ? lessonNameUrlOnline[lessonName]
                : lessonNameUrl[lessonName];
        }

        public static string GetReplacementsUrl(bool online)
        {
            return online
                ? "https://covenant-of-christ.onrender.com/Assets/online/lessons/replacements.json"
                : "/Assets/lessons/replacements.json";
        }

        public static string GetManifestUrl()
        {
            return "https://covenant-of-christ.onrender.com/Assets/lessons/manifest.json";
        }
    }
}
