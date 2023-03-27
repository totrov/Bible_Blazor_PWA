using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Config
{
    public static class LessonLoadConfig
    {
        private static Dictionary<string, string> lessonNameUrl = new()
        {
            {"Бытие", "/Assets/lessons/Byt.docx" },
            {"Исход - Соломон", "/Assets/lessons/IshodSolomon.docx" },
            {"Пророки", "/Assets/lessons/Pror.docx" },
            {"Евангелия", "/Assets/lessons/Evn.docx" },
            {"Деяния - Откровение", "/Assets/lessons/DeyanOtkr.docx" },
            {"Основы веры", "/Assets/lessons/OsnVer.docx" }
        };

        private static Dictionary<string, string> lessonNameUrlOnline = new()
        {
            {"Бытие", "https://covenantofchrist.onrender.com/Assets/online/lessons/Byt.docx" },
            {"Исход - Соломон", "https://covenantofchrist.onrender.com/Assets/online/lessons/IshodSolomon.docx" },
            {"Пророки", "https://covenantofchrist.onrender.com/Assets/online/lessons/Pror.docx" },
            {"Евангелия", "https://covenantofchrist.onrender.com/Assets/online/lessons/Evn.docx" },
            {"Деяния - Откровение", "https://covenantofchrist.onrender.com/Assets/online/lessons/DeyanOtkr.docx" },
            {"Основы веры", "https://covenantofchrist.onrender.com/Assets/online/lessons/OsnVer.docx" }
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
                ? "https://covenantofchrist.onrender.com/Assets/online/lessons/replacements.json"
                : "/Assets/lessons/replacements.json";
        }
        public static string GetNegativeLookaheadsUrl(bool online)
        {
            return online
                ? "https://covenantofchrist.onrender.com/Assets/online/lessons/negativeLookaheads.json"
                : "/Assets/lessons/negativeLookaheads.json";
        }

        public static string GetManifestUrl()
        {
            return "https://covenantofchrist.onrender.com/Assets/online/lessons/manifest.json";
        }
        public static DateTime GetOfflineVersionDate()
        {
            return DateTime.MinValue;
        }
        public static string ToRussianDateFormatString(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
    }
}
