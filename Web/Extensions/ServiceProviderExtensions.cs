using Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddGoogleSheetsService(this IServiceCollection services)
        {
            services.AddSingleton<GoogleSheets>();
        }
    }
}
