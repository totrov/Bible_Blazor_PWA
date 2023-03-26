using System;

namespace Bible_Blazer_PWA.Diagnostics
{
    public class StringLogger : ITimeCounterLogger
    {
        string stringToLogTo;
        private readonly string code;

        public StringLogger(ref string str, string code)
        {
            stringToLogTo = str;
            this.code = code;
        }
        public void Log(TimeSpan elapsed)
        {
            stringToLogTo+= $"{code}: {elapsed.Milliseconds}ms;\r\n";
        }
    }
}
