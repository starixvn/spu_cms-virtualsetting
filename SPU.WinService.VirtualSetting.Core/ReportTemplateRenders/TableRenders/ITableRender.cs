// --------------------------------------------------------------------------
// <copyright file="ITableRender.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2015.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.ReportTemplateRenders.TableRenders
{
    public interface ITableRender
    {
        string Template { get; set; }

        string RenderTable();
    }
}