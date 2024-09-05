//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections;
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

        protected Dictionary<string, JsonData> groupDataMap;

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

        private Dictionary<string, CancellationTokenSource> WriteCancellationTokenSourceMap;

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
            WriteCancellationTokenSourceMap = new Dictionary<string, CancellationTokenSource>();
        }

        public void SetArchiveID(int archiveID)
        {
            _archiveID = archiveID;
        }

        protected bool DataPersistentReadyWork(string groupKey,string dataKey, string dataStr)
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

        private void WriteToDisk(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }
        
        protected async Task WriteToDiskAsync(string filePath, string text)
        {
            //char stream
            
            WriteCancellationTokenSourceMap.TryGetValue(filePath, out var cancelTokenSource);
            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose(); 
            }
            
            using (var sourceStream = new StreamWriter(filePath))
            {
                sourceStream.AutoFlush = false;
                ReadOnlyMemory<char> readOnlyMemory = new ReadOnlyMemory<char>(text.ToCharArray());
                
                cancelTokenSource = CreateCancelTokenSource(filePath);
                await sourceStream.WriteAsync(readOnlyMemory, cancelTokenSource.Token);
                await sourceStream.FlushAsync();
                cancelTokenSource.Dispose();
                WriteCancellationTokenSourceMap.Remove(filePath);
            }
        }
        
        public void DataPersistent(string groupKey, string dataKey, string dataStr)
        {
            bool hasExistedBefore = DataPersistentReadyWork(groupKey, dataKey, dataStr);
            string groupFilePath = GetAndCombineDataFilePath(groupKey);
            WriteToDisk(groupFilePath, groupDataMap[groupKey].ToJson());
            bool isRewriteGroupKeys = hasExistedBefore == false;
            if (isRewriteGroupKeys)
            {
                TryRecordKey(groupKey);
            }
        }

        public void DataPersistent(params DataObject[] dataObjects)
        {
            if (dataObjects.Length == 0)
            {
                return;
            }
            
            HashSet<string> hashSet = new HashSet<string>();
            foreach (var dataObject in dataObjects)
            {
                string key = dataObject.Key;
                var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(key);
                string groupKey = keyTuple.Item1;
                string dataKey = keyTuple.Item2;
                bool hasExistedBefore = DataPersistentReadyWork(groupKey, dataKey, dataObject.GetSerializeData());

                bool isRewriteGroupKeys = hasExistedBefore == false;
                if (isRewriteGroupKeys)
                {
                    TryRecordKey(groupKey);
                }
                hashSet.Add(groupKey);
            }

            foreach (var groupKey in hashSet)
            {
                string groupFilePath = GetAndCombineDataFilePath(groupKey);
                WriteToDisk(groupFilePath, groupDataMap[groupKey].ToJson());
            }
        }

        public async Task DataPersistentAsync(string groupKey, string dataKey, string dataStr, Action complete)
        {
            bool hasExistedBefore = DataPersistentReadyWork(groupKey, dataKey, dataStr);
            string groupFilePath = GetAndCombineDataFilePath(groupKey);
            
            await WriteToDiskAsync(groupFilePath, groupDataMap[groupKey].ToJson());

            bool isRewriteGroupKeys = hasExistedBefore == false;
            if (isRewriteGroupKeys)
            {
                TryRecordKey(groupKey);
            }
            complete?.Invoke();
        }
        
        public virtual async Task DataPersistentAsync(Action complete, params DataObject[] dataObjects)
        {
            if (dataObjects.Length == 0)
            {
                return;
            }
            
            HashSet<string> hashSet = new HashSet<string>();
            foreach (var dataObject in dataObjects)
            {
                string key = dataObject.Key;
                var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(key);
                string groupKey = keyTuple.Item1;
                string dataKey = keyTuple.Item2;
                bool hasExistedBefore = DataPersistentReadyWork(groupKey, dataKey, dataObject.GetSerializeData());
                bool isRewriteGroupKeys = hasExistedBefore == false;
                if (isRewriteGroupKeys)
                {
                    TryRecordKey(groupKey);
                }
                hashSet.Add(groupKey);
            }

            IList<Task> writeTasks = new List<Task>();
            foreach (var groupKey in hashSet)
            {
                string groupFilePath = GetAndCombineDataFilePath(groupKey);
                writeTasks.Add(WriteToDiskAsync(groupFilePath, groupDataMap[groupKey].ToJson()));
            }
            
            await Task.WhenAll(writeTasks);

            complete?.Invoke();
        }

        public virtual void TryRecordKey(string groupKey)
        {
            if (AllGroupKeys != null)
            {
                AllGroupKeys.Add(groupKey);
            }

            ArchiveOperationHelper.RecordKey(_archiveID, groupKey);
        }

        public string Read(string groupKey, string dataKey)
        {
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

        public void Delete(string groupKey, string dataKey)
        {
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
        
        private async Task<(bool,JsonData)> TryGetJsonDataAsync(string groupKey)
        {
            JsonData jsonData = null;
            if (groupDataMap.TryGetValue(groupKey, out jsonData) == false)
            {
                string str = await LoadFromDiskAsync(groupKey);
                if (string.IsNullOrEmpty(str))
                {
                    return (false, null);
                }

                jsonData = JsonMapper.ToObject(str);
                groupDataMap.Add(groupKey, jsonData);
            }

            return (true, jsonData);
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
        
        private async Task<string> LoadFromDiskAsync(string groupKey)
        {
            string filePath = GetAndCombineDataFilePath(groupKey);
            if (File.Exists(filePath) == false)
            {
                return "";
            }
            var sb = new StringBuilder();
            using (var sourceStream = new StreamReader(filePath))
            {
                char[] buffer = new char[50];
                int readLen;
                while ((readLen = await sourceStream.ReadAsync(buffer, 0,buffer.Length)) != 0)
                {
                    sb.Append(buffer, 0, readLen);
                }
            }
            return sb.ToString();
        }

        private async Task<List<string>> LoadAllGroupKeyFromDisk()
        {
            return await ArchiveOperationHelper.GetAllGroupKey();
        }

        protected string GetAndCombineDataFilePath(string groupKey)
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

        public async Task<IDataArchiveSourceWrapper> GetSource()
        {
            if (AllGroupKeys == null)
            {
                AllGroupKeys = await LoadAllGroupKeyFromDisk();
            }

            var sourceWrapper = new DictionaryJsonArchiveSourceWrapper();
            if (AllGroupKeys != null)
            {
                List<Task> readTasks = new List<Task>();
                foreach (var groupKey in AllGroupKeys)
                {
                    readTasks.Add(TryGetJsonDataAsync(groupKey));
                }
                await Task.WhenAll(readTasks);
                sourceWrapper.source = groupDataMap;
            }
            else
            {
                sourceWrapper.source = null;
            }

            return sourceWrapper;
        }

        public async Task CloneTo(IDataArchiveSourceWrapper source)
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
            List<Task> writeTasks = new List<Task>();
            foreach (var pair in groupDataMap)
            {
                string groupKey = pair.Key;
                string filePath = GetAndCombineDataFilePath(groupKey);
                writeTasks.Add(WriteToDiskAsync(filePath, pair.Value.ToJson()));
            }

            await Task.WhenAll(writeTasks);
            RecordAllGroupKeys();
        }

        protected virtual void RecordAllGroupKeys()
        {
            ArchiveOperationHelper.RecordAllGroupKeyWhenClone(_archiveID);
        }

        private CancellationTokenSource CreateCancelTokenSource(string filePath)
        {
            var source = new CancellationTokenSource();
            WriteCancellationTokenSourceMap[filePath] = source;
            return source;
        }

        public void Dispose()
        {
            groupDataMap = null;
            AllGroupKeys = null;
            archiveOperationHelper = null;
            IsActive = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        public async Task DisposeAsync()
        {
            Dispose();
        }
    }
}