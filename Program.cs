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
using Bible_Blazer_PWA.BibleReferenceParse;

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
            builder.Services.AddSingleton(new MenuService());
            var replacer = new Replacer(http);
            builder.Services.AddSingleton(replacer);

            var host = builder.Build();

            var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
            dbFacade.SetJS(jsRuntime);
            await dbParametersFacade.Init();
            bibleService.Init(dbFacade, dbParametersFacade.ParametersModel);
            await host.RunAsync();
        }
    }
}
