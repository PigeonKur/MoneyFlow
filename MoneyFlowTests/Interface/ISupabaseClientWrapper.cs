using MoneyFlowClient.Client.Models;
using Supabase;
using Supabase.Gotrue;

namespace MoneyFlowTests.Interface
{
    public interface ISupabaseClientWrapper
    {
        Supabase.Client GetClient();
    }

    public class SupabaseClientWrapper : ISupabaseClientWrapper
    {
        private readonly Supabase.Client _client;

        public SupabaseClientWrapper()
        {
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false,
                AutoRefreshToken = false
            };
            _client = new Supabase.Client("https://localhost", "anon-key", options);
        }

        public Supabase.Client GetClient() => _client;
    }
}