//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexiArchiveSystem.Assist;
using FlexiArchiveSystem.Setting;

namespace FlexiArchiveSystem
{
    public class DataGroup : IDisposable
    {
        public Dictionary<string, DataObject> dataObjectMap = new Dictionary<string, DataObject>();
        private List<string> dirtyDataObjectKeys = new List<string>();
        private string _groupKey;
        public event Action<string> OnDirtyHandler;
        private IArchiveSetting _ArchiveSetting;

        public DataGroup(string groupKey)
        {
            _groupKey = groupKey;
        }

        internal void InjectArchiveSetting(IArchiveSetting setting)
        {
            _ArchiveSetting = setting;
        }

        public void LoadAll()
        {

        }

        public bool TryGetDataObject(string key, out DataObject dataObject)
        {
            dataObject = GetDataObject(key);
            return dataObject != null;
        }

        public DataObject GetDataObject(string key)
        {
            string fullKey = DataKeyHandler.CombieGroupAndDataKey(_groupKey, key);
            DataObject dataObject = GetCacheDataObject(fullKey);
            if (dataObject == null)
            {
                dataObject = CreateNewDataObject(fullKey);
                dataObjectMap.Add(fullKey, dataObject);
            }

            return dataObject;
        }

        private DataObject GetCacheDataObject(string key)
        {
            dataObjectMap.TryGetValue(key, out DataObject dataObject);
            return dataObject;
        }

        private DataObject CreateNewDataObject(string key)
        {
            DataObject dataObject = new DataObject(key);
            dataObject.OnDirtyHandler += DataObjectHappenDirty;
            dataObject.InjectArchiveSetting(_ArchiveSetting);
            dataObject.Init();
            return dataObject;
        }

        private void DataObjectHappenDirty(string key)
        {
            if (dirtyDataObjectKeys.Contains(key) == false)
            {
                dirtyDataObjectKeys.Add(key);
            }

            OnDirtyHandler?.Invoke(_groupKey);
        }

        public void Delete()
        {
            _ArchiveSetting.DataArchiveOperation.DeleteGroup(_groupKey);
            _ArchiveSetting.DataTypeSystemInfoOperation.DeleteGroup(_groupKey);
        }

        public void Save()
        {
            if (dirtyDataObjectKeys.Count == 0)
            {
                return;
            }

            var dirtyDataObjectList = dirtyDataObjectKeys.Select(GetCacheDataObject).ToArray();
            _ArchiveSetting.DataArchiveOperation.DataPersistent(dirtyDataObjectList);
            
            foreach (var dirtyDataObject in dirtyDataObjectList)
            {
                dirtyDataObject._dataType.Refresh();
                dirtyDataObject.TryToSaveDataSystemInfo();
                dirtyDataObject.CleanDirty();
            }
            
            if (_ArchiveSetting.IsLog)
            {
                Logger.LOG($"数据存档更新 Group: [{_groupKey}] - [{dirtyDataObjectKeys.Count}]条");
            }

            dirtyDataObjectKeys.Clear();
        }
        
        public async Task SaveAsync(Action complete = null)
        {
            if (dirtyDataObjectKeys.Count == 0)
            {
                return;
            }
            
            //TODO : cancel token
            var dirtyDataObjectList = dirtyDataObjectKeys.Select(GetCacheDataObject).ToArray();
            await _ArchiveSetting.DataArchiveOperation.DataPersistentAsync(complete, dirtyDataObjectList);
            
            foreach (var dirtyDataObject in dirtyDataObjectList)
            {
                dirtyDataObject._dataType.Refresh();
                dirtyDataObject.CleanDirty();
            }
            TryToSaveSystemInfo(null, dirtyDataObjectList);
            complete?.Invoke();
           
            if (_ArchiveSetting.IsLog)
            {
                Logger.LOG($"数据存档更新 Group: [{_groupKey}] - [{dirtyDataObjectKeys.Count}]条");
            }

            dirtyDataObjectKeys.Clear();
        }
        
        private void TryToSaveSystemInfo(Action complete, params DataObject[] dataObjects)
        {
#if UNITY_EDITOR
            _ArchiveSetting.DataTypeSystemInfoOperation.DataPersistentAsync(complete, dataObjects);
#else
         if (_ArchiveSetting.IsAllowSaveDataSystemInfoInPlayerDevice)
         {
            _ArchiveSetting.DataTypeSystemInfoOperation.DataPersistentAsync(complete, dataObjects);
         }
#endif
        }

        public void Dispose()
        {
            if (dataObjectMap != null)
            {
                foreach (var pair in dataObjectMap)
                {
                    pair.Value.Dispose();
                }

                dataObjectMap.Clear();
                dataObjectMap = null;
            }

            dirtyDataObjectKeys = null;
            _groupKey = null;
            OnDirtyHandler = null;
        }
    }
}