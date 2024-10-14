//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Reflection;

namespace FlexiArchiveSystem
{
    public interface IDataType
    {
        public event Action OnDirtyHandler;
        public DataTypeSystemInfo SystemInfo { get; }
        public MethodInfo GetWriteDataMethodInfo();
        public void WriteByGenericObject(object genericValue);
        public void Refresh();
        public string Serialize();
        public void Reset();
        public string ToString();
        public void InjectArchiveOperationType(ArchiveOperationType archiveOperationType);
    }
}