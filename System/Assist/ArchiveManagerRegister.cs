//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;
using FlexiArchiveSystem.Setting;

namespace FlexiArchiveSystem.Assist
{
    public class ArchiveManagerRegister
    {
        public static ArchiveManagerRegister instance = new ArchiveManagerRegister();

        private Dictionary<string, IFlexiDataArchiveManager> ArchiveMgrMap = new Dictionary<string, IFlexiDataArchiveManager>();

        public void Register(IFlexiDataArchiveManager mgr)
        {
            string ModuleName = mgr.ArchiveSetting.ModuleName;
            if (string.IsNullOrEmpty(ModuleName))
            {
                Logger.LOG_ERROR("存档系统的 ModuleName 不能为空");
                return;
            }
            ArchiveMgrMap[ModuleName] = mgr;
        }
        
        public void RemoveRegister(IFlexiDataArchiveManager mgr)
        {
            string ModuleName = mgr.ArchiveSetting.ModuleName;
            if (ArchiveMgrMap.ContainsKey(ModuleName) == false)
            {
                return;
            }

            ArchiveMgrMap.Remove(ModuleName);
        }

        public IFlexiDataArchiveManager FindByArchiveSetting(IArchiveSetting archiveSetting)
        {
            ArchiveMgrMap.TryGetValue(archiveSetting.ModuleName, out var mgr);
            return mgr;
        }
    }
}