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
        public void ToSaveDataTypeSystemInfo(string groupKey, string dataKey, DataTypeSystemInfo dataTypeSystemInfo);
        public DataTypeSystemInfo ReadSystemInfo(string groupKey, string dataKey);
        public Type GetTypeOfDataValue(string groupKey, string dataKey);
    }
}