using BlazorWorker.WorkerCore;
using System;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    public class WorkerMessageStub : IWorkerMessageService
    {
        public event EventHandler<string> IncomingMessage;

        public Task PostMessageAsync(string message)
        {
            return Task.CompletedTask;
        }
    }
}
