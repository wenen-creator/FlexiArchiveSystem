//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;

namespace FlexiArchiveSystem
{
    /// <summary>
    /// 存档系统
    /// Please go to Unity-IFlexiDataArchiveManager what is core code;
    /// </summary>
    public abstract partial class IFlexiDataArchiveManager
    {
        protected abstract ArchiveSettingWrapper LoadDataArchiveSettingFromDisk();
    }
}