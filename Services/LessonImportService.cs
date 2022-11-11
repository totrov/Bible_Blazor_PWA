using BlazorWorker.WorkerCore;
using Microsoft.JSInterop;
using System.Threading.Tasks;

using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Services.Parse;
using System.Net.Http;
using System.IO;
using System.Text.Json;
using BlazorWorker.Core;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using System;
using System.Linq;
using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Extensions.JSRuntime;
using BlazorWorker.WorkerBackgroundService;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImportService
    {
        private readonly ICorrector corrector;
        private LessonImporter lessonImporter;
        private string lessonFileName;
        private FileStream file;

        public LessonImportService(
            IWorkerMessageService workerMessageService,
            HttpClient httpClient,
            ICorrector Corrector,
            IJSRuntime jSRuntime)
        {
            WorkerMessageService = workerMessageService;
            corrector = Corrector;
            JSRuntime = jSRuntime;
            DatabaseJSFacade databaseJSFacade = new DatabaseJSFacade();
            databaseJSFacade.SetJS(jSRuntime);
            InterProcessImportHandler handler = new(
                () => { if (!string.IsNullOrEmpty(lessonFileName)) File.Delete(lessonFileName); },
                workerMessageService);
            lessonImporter = new(httpClient, corrector, databaseJSFacade, handler);
        }

        public async Task<string> WriteBytesToLessonFile(string lessonFileName, string serializedBytes)
        {
            this.lessonFileName = lessonFileName;

            file = new FileStream(lessonFileName, FileMode.Create, FileAccess.Write);
            await file.WriteAsync(System.Convert.FromBase64String(serializedBytes));
            file.Close();

            return "workaround for bug in backgroud worker library. Return type is needed";
        }

        public async Task<string> LoadPredefinedLesson(string lessonName)
        {
            await lessonImporter.LoadPredefinedLesson(lessonName);
            return "workaround for bug in backgroud worker library. Return type is needed";
        }
        public async Task<string> LoadLessonFromFile()
        {
            await lessonImporter.LoadLessonFromFile(lessonFileName);
            return "workaround for bug in backgroud worker library. Return type is needed";
        }
        public void NotifyImportCompleted() => lessonImporter.LessonDbImportAwaiter.SetResult();
        public IWorkerMessageService WorkerMessageService { get; }
        public IJSRuntime JSRuntime { get; }
    }

    public static class LessonImportServiceExtensions
    {
        public static async Task<IServiceCollection> AddLessonImportServiceAsync(this IServiceCollection services)
        {
            var workerFactory = services.BuildServiceProvider().GetService<IWorkerFactory>();
            var worker = await workerFactory.CreateAsync();

            var serviceCollectionDependencies = new string[] {
                    "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
#if NET5_0_OR_GREATER
                "System.Diagnostics.Tracing.dll",
#endif
#if NET6_0_OR_GREATER
                "Microsoft.Extensions.DependencyInjection.dll",
#endif
            "System.Security.Cryptography.X509Certificates.dll",
            "System.Runtime.Serialization.Formatters.dll",
            "DocumentFormat.OpenXml.Wordprocessing.dll",
            "BlazorWorker.BackgroundServiceFactory.dll",
            "BlazorWorker.WorkerBackgroundService.dll",
            "DocumentFormat.OpenXml.Packaging.dll",
            "BlazorWorker.WorkerCore.dll",
            "DocumentFormat.OpenXml.dll",
            "DocumentFormat.OpenXml.dll",
            "System.Net.Primitives.dll",
            "System.IO.Compression.dll",
            "System.Xml.XDocument.dll",
            "System.Net.Http.Json.dll",
            "System.Net.Requests.dll",
            "System.Net.Security.dll",
            "System.IO.Packaging.dll",
            "BlazorWorker.Core.dll",
            "System.Net.Http.dll",
            "MudBlazor.dll"
        };

            var startupService = await worker.CreateBackgroundServiceAsync<LessonImportServiceStartup>(wo =>
                wo.AddBlazorWorkerJsRuntime()
                    .AddConventionalAssemblyOfService()
                    .AddHttpClient()
                    .AddAssemblyOf<Microsoft.Extensions.DependencyInjection.ServiceCollection>()
                    .AddAssemblies(serviceCollectionDependencies)
                );
            var lessonImportService = await startupService.CreateBackgroundServiceAsync(startup => startup.Resolve<LessonImportService>());

            services.AddSingleton<IWorkerBackgroundService<LessonImportService>>(lessonImportService);
            return services;
        }
    }
}
