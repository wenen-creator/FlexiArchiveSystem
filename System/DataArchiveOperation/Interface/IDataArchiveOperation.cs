//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Threading.Tasks;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public interface IDataArchiveOperation : IDisposable
    {
        public bool IsValidation { get; }
        public DataArchiveOperationHelper ArchiveOperationHelper { get; }
        public void SetDataArchiveOperationHelper(DataArchiveOperationHelper helper);
        public void Init(int archiveID);
        public void SetArchiveID(int archiveID);
        public void DataPersistent(string key, string dataStr);
        
        public Task DataPersistentAsync(string key, string dataStr, Action complete);
        
        public string Read(string key);
        public Task DeleteAll();
        public void Delete(string key);

        public void DeleteGroup(string groupKey);

        public void TryRecordKey(string groupKey, string dataKey);

        public void TryRemoveAllGroupKey();
    }
}