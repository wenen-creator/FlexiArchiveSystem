//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;

namespace FlexiArchiveSystem
{
    public interface IDataTypeSystemInfoOperation
    {
        public string SystemInfoPath { get; set; }
        public void ToSaveDataTypeSystemInfo(string key, DataTypeSystemInfo dataTypeSystemInfo);
        public DataTypeSystemInfo ReadSystemInfo(string key);
        public Type GetTypeOfDataValue(string key);
    }
}