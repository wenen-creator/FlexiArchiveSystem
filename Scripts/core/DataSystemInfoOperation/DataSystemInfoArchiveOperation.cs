//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
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

        public void ToSaveDataTypeSystemInfo(string key, DataTypeSystemInfo dataTypeSystemInfo)
        {
            string jsonStr = dataTypeSystemInfo.Serialize();
            DataPersistent(key, jsonStr);
        }

        public DataTypeSystemInfo ReadSystemInfo(string key)
        {
            string jsonStr = Read(key);
            return JsonMapper.ToObject<DataTypeSystemInfo>(jsonStr);
        }

        public Type GetTypeOfDataValue(string key)
        {
            DataTypeSystemInfo dataTypeSystemInfo = ReadSystemInfo(key);
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