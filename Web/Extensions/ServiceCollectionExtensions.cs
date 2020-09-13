using Microsoft.Extensions.DependencyInjection;
using GoogleLib;
using Models;

namespace Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGoogleSheetsService(this IServiceCollection services)
        {
            services.AddSingleton<GoogleSheets>();
        }

        public static void AddServiceContext(this IServiceCollection services)
        {
            services.AddScoped<ServiceContext>();
        }
    }
}
