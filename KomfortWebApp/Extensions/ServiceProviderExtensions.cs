using KomfortWebApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KomfortWebApp.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddGoogleSheetsService(this IServiceCollection services)
        {
            services.AddSingleton<GoogleSheets>();
        }
    }
}
