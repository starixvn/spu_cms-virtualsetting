// --------------------------------------------------------------------------
// <copyright file="ITagFinder.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2015.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.MailMasterRenders
{
    using System.Collections.Generic;

    public interface ITagFinder
    {
        string Tag { get; set; }

        IList<int> GetTagPositions(string content);
    }
}