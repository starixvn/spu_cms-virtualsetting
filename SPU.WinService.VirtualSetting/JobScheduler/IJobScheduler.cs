namespace SPU.WinService.VirtualSetting.JobScheduler
{
    using System;

    public interface IJobScheduler
    {
        void Schedule(Type jobType, int durationInSecond);

        void Schedule(Type jobType, string cronException);
    }
}
