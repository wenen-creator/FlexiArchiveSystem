//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.Setting;

namespace FlexiArchiveSystem
{
    public struct ArchiveSettingWrapper
    {
        public readonly string SettingName;
        public readonly IArchiveSetting ArchiveSetting;

        public ArchiveSettingWrapper(string settingName, IArchiveSetting setting)
        {
            SettingName = settingName;
            ArchiveSetting = setting;
        }
    }
}