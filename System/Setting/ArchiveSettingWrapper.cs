//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.Setting;

namespace FlexiArchiveSystem
{
    public struct ArchiveSettingWrapper
    {
        public readonly IArchiveSetting ArchiveSetting;

        public ArchiveSettingWrapper(IArchiveSetting setting)
        {
            ArchiveSetting = setting;
        }
    }
}