//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FlexiArchiveSystem.ArchiveOperation;
using FlexiArchiveSystem.Assist;
using FlexiArchiveSystem.Setting;

namespace FlexiArchiveSystem.Entry
{
    public class DataArchiveContainer
    {
        private Dictionary<string, DataGroup> dataGroupMap = new Dictionary<string, DataGroup>();
        public List<string> dirtyDataGroupList = new List<string>();
        private IArchiveSetting _dataArchiveSetting;

        internal DataArchiveContainer(IArchiveSetting dataArchiveSetting)
        {
            this._dataArchiveSetting = dataArchiveSetting;
        }

        public DataObject GetDataObject(string group_key, string data_key)
        {
            var dataGroup = GetDataGroup(group_key);
            return dataGroup.GetDataObject(data_key);
        }

        public DataGroup GetDataGroup(string group_key)
        {
            DataGroup dataGroupObject = GetCacheDataGroup(group_key);
            if (dataGroupObject == null)
            {
                dataGroupObject = CreateNewDataGroup(group_key);
                dataGroupMap.Add(group_key, dataGroupObject);
            }

            return dataGroupObject;
        }

        private DataGroup GetCacheDataGroup(string key)
        {
            dataGroupMap.TryGetValue(key, out DataGroup dataGroupObject);
            return dataGroupObject;
        }

        private DataGroup CreateNewDataGroup(string key)
        {
            DataGroup dataGroupObject = new DataGroup(key);
            dataGroupObject.OnDirtyHandler += DataGroupHappenDirty;
            dataGroupObject.InjectArchiveSetting(_dataArchiveSetting);
            return dataGroupObject;
        }

        private void DataGroupHappenDirty(string groupKey)
        {
            if (dirtyDataGroupList.Contains(groupKey) == false)
            {
                dirtyDataGroupList.Add(groupKey);
            }
        }

        public void SwitchArchive(int archiveID)
        {
            if (archiveID != _dataArchiveSetting.CurrentArchiveID)
            {
                _dataArchiveSetting.SwitchArchive(archiveID);
                ClearMemoryCache();
            }
        }

        public void Save()
        {
            foreach (var dirtyDataGroup in dirtyDataGroupList)
            {
                DataGroup dataGroupObject = GetCacheDataGroup(dirtyDataGroup);
                dataGroupObject.Save();
            }

            dirtyDataGroupList.Clear();
        }
        
        public async void SaveAsync(Action allComplete)
        {
            //TODO : cancel token
            IList<Task> saveList = new List<Task>();
            foreach (var dirtyDataGroup in dirtyDataGroupList)
            {
                DataGroup dataGroupObject = GetCacheDataGroup(dirtyDataGroup);
                saveList.Add(dataGroupObject.SaveAsync());
            }
            await Task.WhenAll(saveList);
            allComplete?.Invoke();
            dirtyDataGroupList.Clear();
        }
        
        public void SaveGroup(string group_key)
        {
            int index = dirtyDataGroupList.IndexOf(group_key);
            if (index < 0)
            {
                return;
            }
            var groupObject = GetCacheDataGroup(group_key);
            if (groupObject != null)
            {
                groupObject.Save();
                dirtyDataGroupList.RemoveAt(index);
            }
        }
        
        public void SaveGroup(params string[] group_keys)
        {
            foreach (var group_key in group_keys)
            {
                int index = dirtyDataGroupList.IndexOf(group_key);
                if (index < 0)
                {
                    continue;
                }
                var groupObject = GetCacheDataGroup(group_key);
                if (groupObject != null)
                {
                    groupObject.Save();
                    dirtyDataGroupList.RemoveAt(index);
                }
            }
        }

        public void Delete(string group_key, string data_key)
        {
            DataObject dataObject = GetDataObject(group_key, data_key);
            dataObject.Delete();
        }

        public async void DeleteAll()
        {
#if !UNITY_EDITOR
                if (_dataArchiveSetting.IsAllowSaveDataSystemInfoInPlayerDevice)
                {
                    await _dataArchiveSetting.DataTypeSystemInfoOperation.DeleteAll();
                }
#else
            await _dataArchiveSetting.DataTypeSystemInfoOperation.DeleteAll();
#endif

            await _dataArchiveSetting.DataArchiveOperation.DeleteAll();
            
            _dataArchiveSetting.RefreshArchiveOperation();
            ClearMemoryCache();
        }

        public async void InstantiateNewArchive(Action complete = null)
        {
            int nextArchiveID = _dataArchiveSetting.GetNextArchiveID();
            //Clone
            var currentDataArchiveOperation = _dataArchiveSetting.DataArchiveOperation;
            if (currentDataArchiveOperation.IsValidation == false)
            {
                Logger.LOG_WARNING("当前存档无效，将不会进行存档克隆。如有疑问，请检查存档无效的原因。\n" +
                                   "存档无效条件：字节为0||不存在（或许曾经存在过）");
                return;
            }

            IDataArchiveOperation newDataArchiveOperation = null;
            if (currentDataArchiveOperation is ICloneDataArchive iCloneDataArchive)
            {
                var source = await iCloneDataArchive.GetSource();
                var targetArchiveOperation = DataArchiveOperationFactory.CreateArchiveOperationObject(
                    _dataArchiveSetting.ArchiveOperationMode,
                    _dataArchiveSetting.ModuleName, nextArchiveID);
                targetArchiveOperation.Init(_dataArchiveSetting.ModuleName, nextArchiveID);
                ICloneDataArchive target = targetArchiveOperation as ICloneDataArchive;
                if (target != null)
                {
                    targetArchiveOperation.SetDataArchiveOperationHelper(currentDataArchiveOperation
                        .ArchiveOperationHelper);
                    await currentDataArchiveOperation.DisposeAsync();
                    await target.CloneTo(source);
                    newDataArchiveOperation = target as IDataArchiveOperation;
                    await CreateNewSystemInfoCoupleWithArchive(nextArchiveID);
                }
            }
            else
            {
                throw new Exception("ERROR：当前使用的存档方式，不支持多存档共存机制");
            }

            _dataArchiveSetting.DataArchiveOperation = newDataArchiveOperation;
            _dataArchiveSetting.SetArchiveID(nextArchiveID);
            if (_dataArchiveSetting.IsLog) Logger.LOG("克隆存档成功");
            complete?.Invoke();
        }
        
        private async Task CreateNewSystemInfoCoupleWithArchive(int nextArchiveID)
        {
#if !UNITY_EDITOR
            if (_dataArchiveSetting.IsAllowSaveDataSystemInfoInPlayerDevice == false)
            {
                Logger.LOG("不允许保存SystemInfo");
                return;
            }
#endif
            var currentSystemInfoOperation = _dataArchiveSetting.DataTypeSystemInfoOperation;
            var source = await currentSystemInfoOperation.GetSource();
            var newSystemInfo = DataArchiveOperationFactory.CreateArchiveSystemInfoOperationObject(
                _dataArchiveSetting.ArchiveOperationMode, _dataArchiveSetting.ModuleName, 
                nextArchiveID);
            newSystemInfo.Init(_dataArchiveSetting.ModuleName, nextArchiveID);
            newSystemInfo.SetDataArchiveOperationHelper(currentSystemInfoOperation.ArchiveOperationHelper);
            await currentSystemInfoOperation.DisposeAsync();
            await newSystemInfo.CloneTo(source);
            _dataArchiveSetting.DataTypeSystemInfoOperation = newSystemInfo;
        }

        public void ClearMemoryCache()
        {
            if (dataGroupMap != null)
            {
                dataGroupMap.Clear();
            }

            dirtyDataGroupList.Clear();
        }
    }
}
