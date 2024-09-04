//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LitJson;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public class DataSystemInfoArchiveOperation : FileModeDataArchiveOperation, IDataTypeSystemInfoOperation
    {
        public string SystemInfoPath
        {
            get => Path;
            set => Path = value;
        }
        
        public void ToSaveDataTypeSystemInfo(string groupKey, string dataKey, DataTypeSystemInfo dataTypeSystemInfo)
        {
            string jsonStr = dataTypeSystemInfo.Serialize();
            DataPersistent(groupKey, dataKey, jsonStr);
        }

        public DataTypeSystemInfo ReadSystemInfo(string groupKey, string dataKey)
        {
            string jsonStr = Read(groupKey, dataKey);
            return JsonMapper.ToObject<DataTypeSystemInfo>(jsonStr);
        }

        public Type GetTypeOfDataValue(string groupKey, string dataKey)
        {
            DataTypeSystemInfo dataTypeSystemInfo = ReadSystemInfo(groupKey, dataKey);
            return Type.GetType(dataTypeSystemInfo.systemType);
        }

        public override void TryRecordKey(string groupKey, string dataKey)
        {
            //no record
        }

#pragma warning disable CS1998
        public override async Task DeleteAll()
        {
            if (File.Exists(SystemInfoPath))
            {
                File.Delete(SystemInfoPath);
            }

            this.Dispose();
        }
#pragma warning restore CS1998

        public override void TryRemoveAllGroupKey()
        {
            //no handler
        }
    }
}