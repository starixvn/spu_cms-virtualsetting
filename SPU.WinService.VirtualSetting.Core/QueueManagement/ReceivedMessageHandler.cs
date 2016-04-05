// --------------------------------------------------------------------------
// <copyright file="ReceivingMessageHandler.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2016.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.QueueManagement
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Messaging;
    using System.Threading.Tasks;        
    using FD.Lib.QueueManagement.SendingTasks;    

    /// <summary>
    ///     Receiving Message Handler.
    /// </summary>
    public class ReceivingMessageHandler : IReceivingMessageHandler
    {
        private readonly IMessageQueueReceiver _queue;        
        private Task _currentThread;

        public ReceivingMessageHandler()
        {            
            _queue = new MessageQueueReceiver();
            _queue.AttachReceiveCompletedEventHandler(HandleReceivedMessage);
        }

        public ReceivingMessageHandler(IMessageQueueReceiver queue)
        {         
            _queue = queue;
            _queue.AttachReceiveCompletedEventHandler(HandleReceivedMessage);
        }

        [SuppressMessage(
              "StyleCop.CSharp.Nexcel.NexcelCustomRules",
              "SP2101:MethodMustNotContainMoreLinesThan",
              Justification = "Reviewed. Suppression is OK here.")]
        public void HandleReceivedMessage(object sender, ReceiveCompletedEventArgs e)
        {
            using (var msg = _queue.EndReceive(e.AsyncResult))
            {
                var message = msg.Body;
                var id = (int)message;
                var label = msg.Label;

                if (string.IsNullOrEmpty(label))
                {
                    return;
                }

                _currentThread = _currentThread == null
                    ? Task.Factory.StartNew(() => HandleMessage(label, id))
                    : _currentThread.ContinueWith((antecedent) => HandleMessage(label, id));
            }

            _queue.BeginReceive();
        }

        private void HandleMessage(string label, int id)
        {
                        
        }

        public void TriggerReceivingMessage()
        {
            _queue.BeginReceive();
        }

        public void ClearTasks()
        {
            _queue.DeattachReceiveCompletedEventHandler(HandleReceivedMessage);
        }

        private SendingType ConvertLabelIntoSendingType(string label)
        {
            SendingType type;
            var result = Enum.TryParse(label, true, out type);

            return result ? type : SendingType.None;
        }
    }
}