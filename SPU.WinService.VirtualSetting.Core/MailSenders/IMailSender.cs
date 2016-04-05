// --------------------------------------------------------------------------
// <copyright file="IMailSender.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2016.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.MailSenders
{        
    using SPU.WinService.VirtualSetting.Core.MailMasterRenders;

    public interface IMailSender
    {
        IMailMasterRenderAbstractFactory MailMasterRenderAbstractFactory { get; set; }

        IMailClient MailClient { get; set; }

        void SendMail(int mailMasterId);
    }
}