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
    public class DataManagerSample : IFlexiDataArchiveManager
    {
        public static DataManagerSample instance = new DataManagerSample();

        protected override ArchiveSettingWrapper LoadDataArchiveSettingFromDisk()
        {
            return new ArchiveSettingWrapper(Resources.Load<FlexiArchiveSetting>("DataArchiveSettingByGameplay"));
        }
    }
}