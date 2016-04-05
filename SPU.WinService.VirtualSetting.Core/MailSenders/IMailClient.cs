// --------------------------------------------------------------------------
// <copyright file="IMailClient.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2016.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.MailSenders
{
    using Fanex.MailDelivery.Client.Handlers;
    using Fanex.MailDelivery.Client.Models;

    public interface IMailClient
    {
        void Send(Mail mail);
    }

    public class MailClient : IMailClient
    {
        public void Send(Mail mail)
        {
            MailClientHandler.SendMail(mail);         
        }
    }
}