using Bible_Blazer_PWA.Services;
using Bible_Blazer_PWA.Services.Parse;
using BlazorWorker.Extensions.JSRuntime;
using BlazorWorker.WorkerCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace BlazorWorker.Demo.IoCExample
{
    public class MyServiceStartup
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IWorkerMessageService workerMessageService;
        private readonly HttpClient httpClient;

        /// <summary>
        /// The constructor uses the built-in injection for library-native services such as the <see cref="IWorkerMessageService"/>.
        /// </summary>
        /// <param name="workerMessageService"></param>
        public MyServiceStartup(IWorkerMessageService workerMessageService, HttpClient httpClient)
        {
            this.workerMessageService = workerMessageService;
            this.httpClient = httpClient;
            serviceProvider = ServiceCollectionHelper.BuildServiceProviderFromMethod(Configure);
        }

        public T Resolve<T>() =>  serviceProvider.GetService<T>();

        public void Configure(IServiceCollection services)
        {
            var http = httpClient;
            services.AddTransient<IRegexHelper, BibleRegexHelper>()
                    .AddTransient<ICorrector, Corrector>()
                    .AddTransient<IMyServiceDependency, MyServiceDependency>()
                    .AddBlazorWorkerJsRuntime()
                    .AddTransient<LessonImportService>()
                    .AddSingleton(workerMessageService)
                    .AddScoped(sp => http);
        }
    }
}
