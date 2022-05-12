using System;
using System.IO;

namespace Bible_Blazer_PWA.Services.Readers
{
    public class ReaderBuilder
    {
        private readonly string _fileName;
        private readonly Stream _stream;

        public ReaderBuilder(string fileName)
        {
            _fileName = fileName;
        }

        public ReaderBuilder(Stream stream)
        {
            _stream = stream;
        }
        public IDocumentReader GetReader()
        {
            if (_stream != null)
            {
                return new DocxReader(_stream);
            }
            else if (_fileName != null)
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
                        throw new ReaderException("Неверное расширение файла. Поддерживаемые расширения: *.doc и *.docx", null);
                }
                return reader;
            }
            return null;
        }
    }
}
