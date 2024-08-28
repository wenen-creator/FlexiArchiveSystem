//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;

namespace FlexiArchiveSystem
{
    public interface IDataType
    {
        public event Action OnDirtyHandler;
        public event Action<string, DataTypeSystemInfo> OnPersistentHandler;
        public void ToPersistent();
        public void Reset();
        public string ToString();

        public void InjectArchiveOperationType(ArchiveOperationType archiveOperationType);
    }
}