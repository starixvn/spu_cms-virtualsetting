namespace SPU.WinService.VirtualSetting
{
    using System.ServiceProcess;
    using Core.QueueManagement;
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;    

    public partial class VirtualSettingService : ServiceBase
    {        
        public readonly IReceivingMessageHandler ReceivingMessageHandler;

        public VirtualSettingService()
        {
            InitDependencies();
     
            ReceivingMessageHandler = new ReceivingMessageHandler();

            DbSettingProviderManager.Current.Start();
        }

        private void InitDependencies()
        {
            InitializeComponent();
        }

        public VirtualSettingService(IReceivingMessageHandler receivingMessageHandler)
        {
            InitDependencies();
            
            ReceivingMessageHandler = receivingMessageHandler;
        }

        protected override void OnStart(string[] args)
        {
            RunOnStart();
        }

        protected override void OnStop()
        {
            ReceivingMessageHandler.ClearTasks();
            MailClientHandler.Stop();
        }

        protected override void OnShutdown()
        {
            ReceivingMessageHandler.ClearTasks();
        }

        public void RunOnStart()
        {
            Logger.Log.Error("Starting Fraud-Detection-Service service", "Fraud-Detection-Service-UAT");

            MailClientHandler.Start();

            var queue = new MessageQueueReceiver();
            queue.Clear();            

            ReceivingMessageHandler.TriggerReceivingMessage();

            Logger.Log.Error("Loading tasks has done.", "Fraud-Detection-Service-UAT");
        }
    }
}