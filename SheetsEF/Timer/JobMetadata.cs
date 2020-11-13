using System;

namespace SheetsEF.Timer
{
    public class JobMetadata
    {
        public Type JobType { get; }
        public string CronExpression { get; }

        public JobMetadata(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }
    }
}
