using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System.Threading;
using System.Threading.Tasks;

namespace SheetsEF.Timer
{
    public class TimerHostedService : IHostedService
    {
        public IScheduler Scheduler { get; set; }

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly JobMetadata _jobMetadata;

        public TimerHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            JobMetadata jobMetadata)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _jobMetadata = jobMetadata;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            var job = CreateJob(_jobMetadata);
            var trigger = CreateTrigger(_jobMetadata);
            await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static ITrigger CreateTrigger(JobMetadata jobMetadata)
        {
            return TriggerBuilder
                .Create()
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription(jobMetadata.JobType.Name)
                .Build();
        }

        private static IJobDetail CreateJob(JobMetadata jobMetadata)
        {
            return JobBuilder
                .Create(jobMetadata.JobType)
                .WithDescription(jobMetadata.JobType.Name)
                .Build();
        }
    }
}
