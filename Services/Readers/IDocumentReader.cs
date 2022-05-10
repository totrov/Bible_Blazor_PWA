using System;

namespace Bible_Blazer_PWA.Services.Readers
{
    public interface IDocumentReader:IDisposable
    {
        string ReadDocumentToString(out bool successed);
    }
}
