using System;

namespace Bible_Blazer_PWA.Diagnostics
{
    public interface ITimeCounterLogger
    {
        void Log(TimeSpan elapsed);
    }
}