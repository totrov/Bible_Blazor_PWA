using BlazorWorker.WorkerCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorWorker.Demo.IoCExample;

using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using Bible_Blazer_PWA.Services.Readers;
using System.Net.Http;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImportService
    {
        public class Something
        {
            public string Value { get; set; }
        }
        public string Test(TestParameter test)
        {
            StringBuilder sb = new StringBuilder();
            Stack<char> stack = new Stack<char>();
            
            foreach (var ch in test.S)
            {
                stack.Push(ch);
            }

            char character;
            while (stack.TryPop(out character))
            {
                sb.Append(character);
            }
            return sb.ToString();
        }

        private int FiveCalledCounter = 0;
        private readonly ICorrector corrector;

        public LessonImportService(
            IWorkerMessageService workerMessageService,
            IMyServiceDependency aServiceDependency,
            HttpClient httpClient,
            ICorrector Corrector,
            IJSRuntime jSRuntime)
        {
            WorkerMessageService = workerMessageService;
            AServiceDependency = aServiceDependency;
            corrector = Corrector;
            JSRuntime = jSRuntime;
        }

        public async Task<int> Five()
        {
            await this.WorkerMessageService.PostMessageAsync($"aaaaaaa");
            this.FiveCalled?.Invoke(this, FiveCalledCounter++);
            try
            {
                string tst = corrector.ReplaceBookNames("1-е Ин");
                var theNumberOfTheBeast = await this.JSRuntime.InvokeAsync<int>("eval",
                    "(function(){ console.log('my test:" + tst + ";Hello world invoke call from LessonImportService'); return 666; })()");

                Console.WriteLine($"{theNumberOfTheBeast} : The number of the beast");
                return this.AServiceDependency.Five();
            }
            finally
            {
                if (this.FiveCalledCounter > 2)
                {
                    await this.WorkerMessageService.PostMessageAsync($"{nameof(FiveCalledCounter)} has been called more than 2 times: {this.FiveCalledCounter} times!");
                }
            }
        }

        public event EventHandler<int> FiveCalled;
        public IWorkerMessageService WorkerMessageService { get; }
        public IMyServiceDependency AServiceDependency { get; }
        public IJSRuntime JSRuntime { get; }
    }
}
