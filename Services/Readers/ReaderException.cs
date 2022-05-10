using System;

namespace Bible_Blazer_PWA.Services.Readers
{
    public class ReaderException : Exception
    {
        public ReaderException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
