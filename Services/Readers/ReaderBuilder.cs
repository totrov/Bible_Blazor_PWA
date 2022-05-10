using System;
using System.IO;

namespace Bible_Blazer_PWA.Services.Readers
{
    public class ReaderBuilder
    {
        private readonly string _fileName;

        public ReaderBuilder(string fileName)
        {
            _fileName = fileName;
        }
        public IDocumentReader GetReader()
        {
            string extension = Path.GetExtension(_fileName).ToLower();
            IDocumentReader reader = null;
            switch (extension)
            {
                case ".doc":
                    try
                    {
                        reader = new DocReader(_fileName);
                    }
                    catch (Exception ex)
                    {
                        throw new ReaderException(ex.Message, ex);
                    }
                    break;
                case ".docx":
                    try
                    {
                        reader = new DocxReader(_fileName);
                    }
                    catch (Exception ex)
                    {
                        throw new ReaderException(ex.Message, ex);
                    }
                    break;
                default:
                    throw new ReaderException("Wrong extension of file. Only *.doc and *.docx are supported", null);
            }
            return reader;
        }
    }
}
