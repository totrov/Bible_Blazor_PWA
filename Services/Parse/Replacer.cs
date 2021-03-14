using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public static class Replacer
    {
        private static readonly Dictionary<string, string> mapping = new Dictionary<string, string>
        {
            {"Мф.","Мат" },
            {"1-е Кор.","1Кор" },
            {"2-е Кор.","2Кор" },
            {"Мк.","Мар" },
            {"Дн.","Деян" },
            {"1-е Тим","1Тим"},
            {"2-е Тим","2Тим"},
            {"1-е Ин","1Ин"},
            {"2-е Ин","2Ин"},
            {"1-е Пет","1Пет"},
            {"2-е Пет","2Пет"}
        };

        internal static string ReplaceBookNames(string stringToParse)
        {
            return mapping.Aggregate(stringToParse, (str, replacement) => { return str.Replace(replacement.Key, replacement.Value); });
        }
    }
}
