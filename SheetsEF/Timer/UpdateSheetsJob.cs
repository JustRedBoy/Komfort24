using Microsoft.Extensions.Logging;
using Quartz;
using SheetsEF.Models;
using System;
using System.Threading.Tasks;

namespace SheetsEF.Timer
{
    [DisallowConcurrentExecution]
    public class UpdateSheetsJob<T> : IJob where T : ApplicationContextBase
    {
        private readonly ILogger<UpdateSheetsJob<T>> _logger;
        private readonly T _appContext;

        public UpdateSheetsJob(ILogger<UpdateSheetsJob<T>> logger, T appContext)
        {
            _logger = logger;
            _appContext = appContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Update      " + DateTime.Now);
            await _appContext.UpdatingSheetsAsync();
        }
    }
}
