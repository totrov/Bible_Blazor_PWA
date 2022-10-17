using Bible_Blazer_PWA.Services.Parse;
using BlazorWorker.Demo.IoCExample;
using BlazorWorker.Extensions.JSRuntime;
using BlazorWorker.WorkerCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImportServiceStartup
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IWorkerMessageService workerMessageService;
        private readonly HttpClient httpClient;

        public LessonImportServiceStartup(IWorkerMessageService workerMessageService, HttpClient httpClient)
        {
            this.workerMessageService = workerMessageService;
            this.httpClient = httpClient;
            serviceProvider = ServiceCollectionHelper.BuildServiceProviderFromMethod(Configure);
        }

        public T Resolve<T>() => serviceProvider.GetService<T>();

        public void Configure(IServiceCollection services)
        {
            var http = httpClient;
            services.AddTransient<IRegexHelper, BibleRegexHelper>()
                    .AddTransient<ICorrector, Corrector>()
                    .AddBlazorWorkerJsRuntime()
                    .AddTransient<LessonImportService>()
                    .AddSingleton(workerMessageService)
                    .AddScoped(sp => http);
        }
    }
}
