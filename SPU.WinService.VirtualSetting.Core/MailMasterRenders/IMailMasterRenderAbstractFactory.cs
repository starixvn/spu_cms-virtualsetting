// --------------------------------------------------------------------------
// <copyright file="IMailMasterRenderAbstractFactory.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2015.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.MailMasterRenders
{    
    using FD.Lib.ReportTemplateRenders;

    public interface IMailMasterRenderAbstractFactory
    {
        IMailMasterRender GetMailMasterRender();
    }

    public class MailMasterRenderAbstractFactory : IMailMasterRenderAbstractFactory
    {
        public IReportRenderAbstractFactory ReportRenderFactory { get; set; }

        public IMailMasterRender GetMailMasterRender()
        {
            return new MailMasterRender(ReportRenderFactory);
        }
    }
}