//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Threading.Tasks;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public interface ICloneDataArchive
    {
        public Task<IDataArchiveSourceWrapper> GetSource();
        public Task CloneTo(IDataArchiveSourceWrapper source);
    }
}