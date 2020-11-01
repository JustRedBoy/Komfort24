using GoogleLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseServiceContext(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                GoogleSheets googleSheets = context.RequestServices.GetService<GoogleSheets>();
                ServiceContext serviceContext = context.RequestServices.GetService<ServiceContext>();
                await serviceContext.InitContextAsync(googleSheets);
                await next.Invoke();
            });
        }
    }
}
