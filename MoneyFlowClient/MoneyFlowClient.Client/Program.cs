using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoneyFlowClient.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp =>
             new HttpClient { BaseAddress = new Uri("http://localhost:5188/") });


            await builder.Build().RunAsync();
        }
    }
}
