//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

namespace FlexiArchiveSystem.ArchiveOperation
{
    public interface ICloneDataArchive
    {
        public IDataArchiveSourceWrapper GetSource();
        public void CloneTo(IDataArchiveSourceWrapper source);
    }
}