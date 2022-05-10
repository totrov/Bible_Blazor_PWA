using b2xtranslator.StructuredStorage.Reader;
using System;

namespace Bible_Blazer_PWA.Services.Readers
{
    public class DocReader : IDocumentReader
    {
        StructuredStorageReader reader;

        public DocReader(string name)
        {
            reader = new StructuredStorageReader(name);
        }

        public void Dispose()
        {
            if (reader != null)
                reader.Dispose();
        }

        public string ReadDocumentToString(out bool successed)
        {
            try
            {
                
                b2xtranslator.DocFileFormat.WordDocument doc = new(reader);
                string stringContent = new String(doc.Text.ToArray());
                successed = true;
                return stringContent;
            }
            catch (b2xtranslator.DocFileFormat.ByteParseException e)
            {
                successed = false;
                throw new ReaderException("Ошибка чтения документа. Поддерживается только *.doc.", e);
            }
            catch (b2xtranslator.StructuredStorage.Common.InvalidValueInHeaderException e)
            {
                successed = false;
                throw new ReaderException("Ошибка при попытке открыть файл.", e);
            }
            catch (Exception ex)
            {
                successed = false;
                throw new ReaderException($"Ошибка: {ex.Message}", ex);
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }
        }
    }
}
