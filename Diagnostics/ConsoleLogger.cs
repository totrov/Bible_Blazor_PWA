using Bible_Blazer_PWA.DataBase;
using System;

namespace Bible_Blazer_PWA.Diagnostics
{
    public class ConsoleLogger : ITimeCounterLogger
    {
        private readonly DatabaseJSFacade db;
        private readonly string code;

        public ConsoleLogger(Bible_Blazer_PWA.DataBase.DatabaseJSFacade db, string code)
        {
            this.db = db;
            this.code = code;
        }
        public void Log(TimeSpan elapsed)
        {
            db.JSLog($"{code}: {elapsed.Milliseconds}ms;");
        }
    }
}
