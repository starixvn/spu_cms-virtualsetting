namespace SPU.WinService.VirtualSetting.JobScheduler
{
    public interface IQuartzJobScheduler
    {
        void Shutdown();

        void Start();

        void PauseAll();

        void ResumeAll();
    }
}
