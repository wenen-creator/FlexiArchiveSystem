﻿//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.Setting;
using UnityEngine;

namespace FlexiArchiveSystem.Sample
{
    public class DataManagerSample_2 : IFlexiDataArchiveManager
    {
        public static DataManagerSample_2 instance = new DataManagerSample_2();

        protected override ArchiveSettingWrapper LoadDataArchiveSettingFromDisk()
        {
            return new ArchiveSettingWrapper(Resources.Load<FlexiArchiveSetting>("DataArchiveSettingBySetting"));
        }
    }
}