//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public interface IDataArchiveOperation : IDisposable
    {
        public string ArchiveSystemName { get; } 
        public bool IsValidation { get; }
        public DataArchiveOperationHelper ArchiveOperationHelper { get; }
        public void SetDataArchiveOperationHelper(DataArchiveOperationHelper helper);
        public void Init(string moudleName,int archiveID);
        public void SetArchiveID(int archiveID);
        public void DataPersistent(string groupKey, string dataKey, string dataStr);
        
        public void DataPersistent(params DataObject[] dataObjects);

        public Task DataPersistentAsync(string groupKey, string dataKey, string dataStr, Action complete);

        public Task DataPersistentAsync(Action complete, params DataObject[] dataObjects);
        
        public string Read(string groupKey, string dataKey);
        public Task DeleteAll();
        public void Delete(string groupKey, string dataKey);

        public void DeleteGroup(string groupKey);

        public Task DisposeAsync();

        public void TryRecordKey(string groupKey);

        public void TryRemoveAllGroupKey();
    }
}