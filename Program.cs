using Bible_Blazer_PWA.DataBase;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Bible_Blazer_PWA.Shared;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Web;

namespace Bible_Blazer_PWA
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices();

            var bibleService = new BibleService();
            builder.Services.AddSingleton(bibleService);

            var dbFacade = new DatabaseJSFacade();
            builder.Services.AddSingleton(dbFacade);
            var dbParametersFacade = new Parameters.DbParametersFacade(dbFacade);
            builder.Services.AddSingleton(dbParametersFacade);

            var host = builder.Build();

            var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
            dbFacade.SetJS(jsRuntime);
            bibleService.Init(dbFacade);
            await dbParametersFacade.Init();
            await host.RunAsync();
        }
    }
}
