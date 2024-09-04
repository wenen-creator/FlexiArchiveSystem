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
        
        public async void ToAsyncSaveDataTypeSystemInfo(string groupKey, string dataKey, DataTypeSystemInfo dataTypeSystemInfo)
        {
            string jsonStr = dataTypeSystemInfo.Serialize();
            await DataPersistentAsync(groupKey, dataKey, jsonStr, null);
        }

        public bool ReadSystemInfo(string groupKey, string dataKey, out DataTypeSystemInfo systemInfo)
        {
            string jsonStr = Read(groupKey, dataKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                systemInfo = default(DataTypeSystemInfo);
                return false;
            }
            systemInfo = JsonMapper.ToObject<DataTypeSystemInfo>(jsonStr);
            return true;
        }

        public Type GetTypeOfDataValue(string groupKey, string dataKey)
        {
            bool success = ReadSystemInfo(groupKey, dataKey,out DataTypeSystemInfo dataTypeSystemInfo);
            if (success == false)
            {
                return null;
            }
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