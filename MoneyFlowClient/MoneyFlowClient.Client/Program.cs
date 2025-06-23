using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Supabase;

namespace MoneyFlowClient.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri("http://localhost:5188/") });

            // Конфигурация Supabase
            builder.Services.AddScoped(provider =>
            {
                var url = "https://ymzpnhfzzfkopqvrudiz.supabase.co";
                var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InltenBuaGZ6emZrb3BxdnJ1ZGl6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA2NzU3NTgsImV4cCI6MjA2NjI1MTc1OH0.kdI_yO5Ntlr6ppat40765-Fxhr6C1ANOFJ6mAKFKGcY";

                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true

                };

                return new Supabase.Client(url, key, options);
            });

            await builder.Build().RunAsync();
        }
    }
}