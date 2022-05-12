namespace Bible_Blazer_PWA.Services.Readers
{
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using System;
    using System.IO;
    using System.Text;

    public class DocxReader : IDocumentReader
    {
        WordprocessingDocument wordDocument;
        private Stream _stream;

        public DocxReader(string fileName)
        {
            try
            {
                wordDocument = WordprocessingDocument.Open(fileName, false);
            }
            catch (Exception ex)
            {
                throw new ReaderException(ex.Message, ex);
            }
        }

        public DocxReader(Stream stream)
        {
            _stream = stream;
            try
            {
                wordDocument = WordprocessingDocument.Open(stream, false);
            }
            catch (Exception ex)
            {
                throw new ReaderException(ex.Message, ex);
            }
        }

        public void Dispose()
        {
            if (wordDocument != null)
                wordDocument.Dispose();
            if (_stream != null)
                _stream.Close();
        }

        public string ReadDocumentToString(out bool successed)
        {
            successed = false;
            string result = "";
            try
            {
                var sb = new StringBuilder();
                foreach (var p in wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>())
                {
                    sb.Append(p.InnerText);
                    sb.Append("\r");
                }

                result = sb.ToString();
                successed = true;
            }
            catch (Exception ex)
            {
                successed = false;
                throw new ReaderException(ex.Message, ex);
            }
            return result;
        }
    }
}
