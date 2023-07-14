using Bible_Blazer_PWA.DataBase;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Bible_Blazer_PWA.Services.Menu;
using Bible_Blazer_PWA.Services.Parse;
using Bible_Blazer_PWA.Services;
using Bible_Blazer_PWA.Facades;

namespace Bible_Blazer_PWA
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            builder.Services.AddScoped(sp => http);
            builder.Services.AddMudServices();

            var bibleService = new BibleService();
            builder.Services.AddSingleton(bibleService);

            var dbFacade = new DatabaseJSFacade();
            builder.Services.AddSingleton(dbFacade);
            var dbParametersFacade = new Parameters.DbParametersFacade(dbFacade);
            builder.Services.AddSingleton(dbParametersFacade);
            builder.Services.AddScoped<HttpFacade>();
            builder.Services.AddScoped<LessonUpdater>();
            builder.Services.AddSingleton(new MenuService());
            builder.Services.AddSingleton<ImportExportService>();
            //var jsInteropService = new JSInteropService();
            //builder.Services.AddSingleton(jsInteropService);

            var regexHelper = new BibleRegexHelper(http);
            builder.Services.AddSingleton(regexHelper);
            await regexHelper.Init();

            builder.Services.AddSingleton(new Corrector(regexHelper));

            var host = builder.Build();

            var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
            dbFacade.SetJS(jsRuntime);
            await dbParametersFacade.Init();
            bibleService.Init(dbFacade, dbParametersFacade.ParametersModel);
            //await jsInteropService.Init(jsRuntime);

            await host.RunAsync();
        }
    }
}