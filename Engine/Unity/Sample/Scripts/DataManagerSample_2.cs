//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem.Sample
{
    public class DataManagerSample_2 : IFlexiDataArchiveManager
    {
        public static DataManagerSample_2 instance = new DataManagerSample_2();

        protected override ArchiveSettingWrapper LoadDataArchiveSettingFromDisk()
        {
            return new ArchiveSettingWrapper("DataArchiveSettingByGameplay",
                Resources.Load<FlexiArchiveSetting>("DataArchiveSettingByFile"));
        }
    }
}