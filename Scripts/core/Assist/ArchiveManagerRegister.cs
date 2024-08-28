//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;

namespace FlexiArchiveSystem.Assist
{
    public class ArchiveManagerRegister
    {
        public static ArchiveManagerRegister instance = new ArchiveManagerRegister();

        private List<IFlexiDataArchiveManager> ArchiveMgrCollections = new List<IFlexiDataArchiveManager>();

        public void Register(IFlexiDataArchiveManager mgr)
        {
            if (ArchiveMgrCollections.Contains(mgr) == false)
            {
                ArchiveMgrCollections.Add(mgr);
            }
        }

        public IFlexiDataArchiveManager FindByArchiveSetting(IArchiveSetting archiveSetting)
        {
            for (int i = 0; i < ArchiveMgrCollections.Count; i++)
            {
                if (ArchiveMgrCollections[i].ArchiveSetting.name == archiveSetting.Name)
                {
                    return ArchiveMgrCollections[i];
                }
            }

            return null;
        }
    }
}