//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LitJson;

namespace FlexiArchiveSystem.ArchiveOperation
{
    /// <summary>
    /// 文件全量形式的存档方式
    /// </summary>
    public class FileModeDataArchiveOperation : IDataArchiveOperation, ISetDataArchivePath, ICloneDataArchive
    {
        public bool IsValidation
        {
            get { return IsActive && Directory.Exists(Path); }
        }

        private bool IsActive;

        public string Path { get; set; }

        private string FilePath;

        private Dictionary<string, JsonData> groupDataMap;

        public List<string> AllGroupKeys;

        private int _archiveID;
        private DataArchiveOperationHelper archiveOperationHelper;

        public DataArchiveOperationHelper ArchiveOperationHelper
        {
            get
            {
                if (archiveOperationHelper == null)
                {
                    SetDataArchiveOperationHelper(DataArchiveOperationFactory.GetDataArchiveOperationHelper());
                }

                return archiveOperationHelper;
            }
        }

        public void SetDataArchiveOperationHelper(DataArchiveOperationHelper helper)
        {
            helper.SetArchiveID(_archiveID);
            archiveOperationHelper = helper;
        }

        public void Init(int archiveID)
        {
            SetArchiveID(archiveID);
            groupDataMap = new Dictionary<string, JsonData>();
            IsActive = true;
        }

        public void SetArchiveID(int archiveID)
        {
            _archiveID = archiveID;
        }

        private bool DataPersistentReadyWork(string groupKey,string dataKey, string dataStr)
        {
            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }

            bool hasExistedBefore = TryGetJsonData(groupKey, out JsonData jsonData);
            
            if (hasExistedBefore == false)
            {
                jsonData = new JsonData();
                groupDataMap.Add(groupKey, jsonData);
            }
            
            jsonData[dataKey] = JsonMapper.ToObject(dataStr);
            return hasExistedBefore;
        }
        
        public void DataPersistent(string key, string dataStr)
        {
            var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(key);
            string groupKey = keyTuple.Item1;
            string dataKey = keyTuple.Item2;
            bool hasExistedBefore = DataPersistentReadyWork(groupKey, dataKey, dataStr);
            string groupFilePath = GetAndCombineDataFilePath(groupKey);
            File.WriteAllText(groupFilePath, groupDataMap[groupKey].ToJson());
            bool isRewriteGroupKeys = hasExistedBefore == false;
            if (isRewriteGroupKeys)
            {
                TryRecordKey(groupKey, dataKey);
            }
        }

        public async Task DataPersistentAsync(string key, string dataStr, Action complete)
        {
            var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(key);
            string groupKey = keyTuple.Item1;
            string dataKey = keyTuple.Item2;
            bool hasExistedBefore = DataPersistentReadyWork(groupKey, dataKey, dataStr);
            string groupFilePath = GetAndCombineDataFilePath(groupKey);

            await File.WriteAllTextAsync(groupFilePath, groupDataMap[groupKey].ToJson());
            
            bool isRewriteGroupKeys = hasExistedBefore == false;
            if (isRewriteGroupKeys)
            {
                TryRecordKey(groupKey, dataKey);
            }
            complete?.Invoke();
        }


        public virtual void TryRecordKey(string groupKey, string dataKey)
        {
            if (AllGroupKeys != null)
            {
                AllGroupKeys.Add(groupKey);
            }

            ArchiveOperationHelper.RecordKey(_archiveID, groupKey, dataKey);
        }

        public string Read(string key)
        {
            var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(key);
            string groupKey = keyTuple.Item1;
            string dataKey = keyTuple.Item2;
            if (Directory.Exists(Path) == false)
            {
                return "";
            }

            TryGetJsonData(groupKey, out JsonData jsonData);
            string jsonResult = "";
            try
            {
                if (jsonData != null)
                {
                    JsonData concreteData_JsonData = jsonData[dataKey];
                    jsonResult = concreteData_JsonData == null
                        ? ""
                        : concreteData_JsonData.ToJson();
                }
            }
            catch (KeyNotFoundException)
            {
                jsonResult = "";
            }

            return jsonResult;
        }

#pragma warning disable CS1998
        public virtual async Task DeleteAll()
        {
            if (Directory.Exists(DataArchiveConstData.GetArchiveDirectoryPath(_archiveID)))
            {
                Directory.Delete(DataArchiveConstData.GetArchiveDirectoryPath(_archiveID), true);
            }

            TryRemoveAllGroupKey();
            this.Dispose();
        }
#pragma warning restore CS1998

        public virtual void TryRemoveAllGroupKey()
        {
            ArchiveOperationHelper.DeleteAllGroupKeyFromDisk();
        }

        public void Delete(string key)
        {
            var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(key);
            string groupKey = keyTuple.Item1;
            string dataKey = keyTuple.Item2;
            bool isGet = TryGetJsonData(groupKey, out JsonData jsonData);
            if (isGet == false)
            {
                return;
            }

            jsonData[dataKey] = null;
            string groupFilePath = GetAndCombineDataFilePath(groupKey);
            File.WriteAllText(groupFilePath, jsonData.ToJson());
        }

        public void DeleteGroup(string groupKey)
        {
            string groupFilePath = GetAndCombineDataFilePath(groupKey);
            if (File.Exists(groupFilePath))
            {
                File.Delete(groupFilePath);
                ArchiveOperationHelper.RemoveGroupKey(groupKey);
            }
        }

        private bool TryGetJsonData(string groupKey, out JsonData jsonData)
        {
            jsonData = null;
            if (groupDataMap.TryGetValue(groupKey, out jsonData) == false)
            {
                string str = LoadFromDisk(groupKey);
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }

                jsonData = JsonMapper.ToObject(str);
                groupDataMap.Add(groupKey, jsonData);
            }

            return true;
        }

        private string LoadFromDisk(string groupKey)
        {
            string filePath = GetAndCombineDataFilePath(groupKey);
            if (File.Exists(filePath) == false)
            {
                return "";
            }

            string str = File.ReadAllText(filePath, Encoding.UTF8);
            return str;
        }

        private List<string> LoadAllGroupKeyFromDisk()
        {
            return ArchiveOperationHelper.GetAllGroupKey();
        }

        private string GetAndCombineDataFilePath(string groupKey)
        {
            string groupFilePath = DataArchiveConstData.GetAndCombineDataFilePath(Path, groupKey);
            return groupFilePath;
        }

        public Dictionary<string, JsonData> ConvertToDictionary(string groupKey)
        {
            string groupFilePath = GetAndCombineDataFilePath(groupKey);
            TryGetJsonData(groupKey, out JsonData jsonData);
            return JsonMapper.ToObject<Dictionary<string, JsonData>>(jsonData.ToJson());
        }

        public IDataArchiveSourceWrapper GetSource()
        {
            if (AllGroupKeys == null)
            {
                AllGroupKeys = LoadAllGroupKeyFromDisk();
            }

            var sourceWrapper = new DictionaryJsonArchiveSourceWrapper();
            if (AllGroupKeys != null)
            {
                foreach (var groupKey in AllGroupKeys)
                {
                    TryGetJsonData(groupKey, out JsonData jsonData);
                }

                sourceWrapper.source = groupDataMap;
            }
            else
            {
                sourceWrapper.source = null;
            }

            return sourceWrapper;
        }

        public void CloneTo(IDataArchiveSourceWrapper source)
        {
            if (source is DictionaryJsonArchiveSourceWrapper == false)
            {
                throw new Exception("克隆Source错误");
            }

            var jsonSource = source as DictionaryJsonArchiveSourceWrapper;
            groupDataMap = jsonSource.source;
            if (groupDataMap == null || groupDataMap.Count == 0)
            {
                return;
            }

            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }

            foreach (var pair in groupDataMap)
            {
                string groupKey = pair.Key;
                string filePath = GetAndCombineDataFilePath(groupKey);
                File.WriteAllText(filePath, pair.Value.ToJson());
            }

            ArchiveOperationHelper.RecordAllGroupKey(_archiveID, groupDataMap.Keys.ToList());
        }

        public void Dispose()
        {
            groupDataMap = null;
            AllGroupKeys = null;
            archiveOperationHelper = null;
            IsActive = false;
        }
    }
}