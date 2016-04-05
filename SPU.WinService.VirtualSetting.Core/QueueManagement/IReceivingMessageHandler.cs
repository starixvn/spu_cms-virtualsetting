// --------------------------------------------------------------------------
// <copyright file="IReceivingMessageHandler.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2016.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.QueueManagement
{
    using System.Messaging;

    /// <summary>
    /// Receiving Message Handler.
    /// </summary>
    public interface IReceivingMessageHandler
    {
        void HandleReceivedMessage(object sender, ReceiveCompletedEventArgs e);

        void TriggerReceivingMessage();

        void ClearTasks();
    }
}