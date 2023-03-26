using System;
using System.Diagnostics;

namespace Bible_Blazer_PWA.Diagnostics
{
    public class TimeCounter : IDisposable
    {
        private readonly ITimeCounterLogger logger;
        Stopwatch stopwatch;

        public TimeCounter(ITimeCounterLogger logger)
        {
            this.logger = logger;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public TimeCounter(ref string strToLogTo, string code) : this(new StringLogger(ref strToLogTo, code)) { }
        public TimeCounter(DataBase.DatabaseJSFacade db, string code) : this(new ConsoleLogger(db, code)) { }

        public void Dispose()
        {
            stopwatch.Stop();
            logger.Log(stopwatch.Elapsed);
        }
    }
}
