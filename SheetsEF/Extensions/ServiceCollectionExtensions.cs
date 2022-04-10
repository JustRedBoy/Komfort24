using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using SheetsEF.Models;
using SheetsEF.Timer;

namespace SheetsEF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSheetsContext<T>(this IServiceCollection services, string updateTime) where T : ApplicationContextBase
        {
            services.AddSingleton<T>();

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<UpdateSheetsJob<T>>();
            services.AddSingleton(new JobMetadata(typeof(UpdateSheetsJob<T>), updateTime));

            services.AddHostedService<TimerHostedService>();
        }
    }
}
