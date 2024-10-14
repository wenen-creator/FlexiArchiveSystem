//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.Setting;
using UnityEngine;

namespace FlexiArchiveSystem
{
    /// <summary>
    /// Flexi Archive System 为你准备了一个默认的存档系统实例
    /// Flexi Archive System provides you with a default archive system instance
    /// </summary>
    public sealed class DefaultDataArchiveManager : IFlexiDataArchiveManager
    {
        private static DefaultDataArchiveManager instance;

        public static DefaultDataArchiveManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DefaultDataArchiveManager();
                    // instance.Init();
                }

                return instance;
            }
        }
        protected override ArchiveSettingWrapper LoadDataArchiveSettingFromDisk()
        {
            return new ArchiveSettingWrapper(
                Resources.Load<FlexiArchiveSetting>("FlexiAsset/DefaultDataArchiveSetting"));
        }
    }
}
