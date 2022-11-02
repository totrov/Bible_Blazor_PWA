using System;

namespace Bible_Blazer_PWA.Services
{
    public class WebWorkerMessageParser
    {
        public event Action<string> OnDBPut;
        public event Action<string> OnInfo;
        public event Action<string> OnWarning;
        public event Action<string> OnError;
        public event Action<string> OnMessage;
        public event Action<string> OnSuccess;
        public void ParseMessage(string message)
        {
            if (message.Length >= 20)
            {
                var type = message[..19].Trim();
                var messageBody = message[20..];
                switch (type)
                {
                    case "DB_PUT":
                        OnDBPut?.Invoke(messageBody);
                        break;
                    case "INFO":
                        OnInfo?.Invoke(messageBody);
                        break;
                    case "WARNING":
                        OnWarning?.Invoke(messageBody);
                        break;
                    case "ERROR":
                        OnError?.Invoke(messageBody);
                        break;
                    case "MESSAGE":
                        OnMessage?.Invoke(messageBody);
                        break;
                    case "SUCCESS":
                        OnSuccess?.Invoke(messageBody);
                        break;
                }
            }
        }
    }
}
