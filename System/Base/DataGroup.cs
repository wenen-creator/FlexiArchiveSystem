//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlexiArchiveSystem.Assist;

namespace FlexiArchiveSystem
{
    public class DataGroup : IDisposable
    {
        public Dictionary<string, DataObject> dataObjectMap = new Dictionary<string, DataObject>();
        private List<string> dirtyDataObjectList = new List<string>();
        private string _groupKey;
        public event Action<string> OnDirtyHandler;
        private IArchiveSetting _ArchiveSetting;

        public DataGroup(string groupKey)
        {
            _groupKey = groupKey;
        }

        public void InjectArchiveSetting(IArchiveSetting setting)
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
            if (dirtyDataObjectList.Contains(key) == false)
            {
                dirtyDataObjectList.Add(key);
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
            foreach (var dirtyDataObject in dirtyDataObjectList)
            {
                DataObject dataObject = GetCacheDataObject(dirtyDataObject);
                dataObject.Save();
            }

            if (dirtyDataObjectList.Count > 0 && _ArchiveSetting.IsLog)
            {
                Logger.LOG($"数据存档更新[{dirtyDataObjectList.Count}]条");
            }

            dirtyDataObjectList.Clear();
        }
        
        public async Task SaveAsync(Action complete = null)
        {
            //TODO : cancel token
            IList<Task> saveList = new List<Task>();
            foreach (var dirtyDataObject in dirtyDataObjectList)
            {
                DataObject dataObject = GetCacheDataObject(dirtyDataObject);
                saveList.Add(dataObject.SaveAsync());
            }

            await Task.WhenAll(saveList);
            complete?.Invoke();
           
            if (dirtyDataObjectList.Count > 0 && _ArchiveSetting.IsLog)
            {
                Logger.LOG($"数据存档更新[{dirtyDataObjectList.Count}]条");
            }

            dirtyDataObjectList.Clear();
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

            dirtyDataObjectList = null;
            _groupKey = null;
            OnDirtyHandler = null;
        }
    }
}