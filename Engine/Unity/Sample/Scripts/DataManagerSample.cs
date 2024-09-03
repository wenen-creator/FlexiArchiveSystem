//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem.Sample
{
    public class DataManagerSample : IFlexiDataArchiveManager
    {
        public static DataManagerSample instance = new DataManagerSample();

        protected override ArchiveSettingWrapper LoadDataArchiveSettingFromDisk()
        {
            return new ArchiveSettingWrapper("DataArchiveSettingByGameplay",
                Resources.Load<FlexiArchiveSetting>("DataArchiveSettingByGameplay"));
        }
    }
}