// --------------------------------------------------------------------------
// <copyright file="MailSender.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2016.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.MailSenders
{
    using Fanex.MailDelivery.Client.Models;
    using SPU.WinService.VirtualSetting.Core.MailMasterRenders;

    public class MailSender : IMailSender
    {
        public MailSender()
        {
            MailClient = new MailClient();
        }

        public MailSender(IMailClient client)
        {
            MailClient = client;
        }

        public IMailMasterRenderAbstractFactory MailMasterRenderAbstractFactory { get; set; }

        public IMailClient MailClient { get; set; }

        public void SendMail(int mailMasterId)
        {
            var mailMasterRender = MailMasterRenderAbstractFactory.GetMailMasterRender();
            var mail = mailMasterRender.RenderContentOfMailMaster(mailMasterId);

            var newMail = new Mail
            {
                Subject = mail.MailSubject,
                Title = mail.MailSubject,
                Body = mail.MailContent,
                MailingListId = mail.MailingListId
            };

            MailClient.Send(newMail);
        }
    }
}