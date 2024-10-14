//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlexiArchiveSystem.ArchiveOperation.IO;
using FlexiArchiveSystem.Assist;
using LitJson;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public class DataArchiveOperationHelper
    {
        private string _ArchiveSystemName;
        private List<string> GroupKeys;
        private JsonData groupKeysJsonData;
        private bool GroupKeysJsonDataIsDirty => groupKeysJsonData == null;

        private int _archiveID = -1;
        
        private Dictionary<int, CancellationTokenSource> WriteCancellationTokenSourceMap = new Dictionary<int, CancellationTokenSource>();

        public void SetArchiveID(int archiveID)
        {
            _archiveID = archiveID;
        }

        public void Init(string ArchiveSystemName)
        {
            _ArchiveSystemName = ArchiveSystemName;
        }
        
        public void RecordKey(int archiveID, string groupKey)
        {
            UpdateDirtyState(archiveID);
            //记录key
            TryAddGroupKey(groupKey);
        }
        
        public void RecordAllGroupKey(int archiveID, List<string> groupKeys)
        {
            UpdateDirtyState(archiveID);
            //记录key
            GroupKeys = groupKeys;
            groupKeysJsonData = ConvertToJsonData(groupKeys);
            WriteToDisk(groupKeysJsonData);
        }
        
        /// <summary>
        /// 必须保证前后使用的helper是一致的，且数据为更改
        /// </summary>
        /// <param name="archiveID"></param>
        public async void RecordAllGroupKeyWhenClone(int archiveID)
        {
            if (GroupKeysJsonDataIsDirty)
            {
                //cache
                await GetAllGroupKey();
            }
            SetArchiveID(archiveID);
            WriteToDisk(groupKeysJsonData);
        }


        public void UpdateDirtyState(int archiveID)
        {
            if (archiveID != _archiveID)
            {
                SetArchiveID(archiveID);
                groupKeysJsonData = null; //GroupKeysJsonDataIsDirty is true
            }
        }

        private async void TryAddGroupKey(string groupKey)
        {
            var groupKeys = await GetAllGroupKey();
            if (groupKeys == null)
            {
                groupKeys = new List<string>();
            }

            foreach (var key in groupKeys)
            {
                if (key == groupKey)
                {
                    return;
                }
            }

            groupKeys.Add(groupKey);
            GroupKeys = groupKeys;
            groupKeysJsonData.Add(groupKey);
            await WriteAysncToDisk(groupKeysJsonData);
        }

        public async Task<List<string>> GetAllGroupKey()
        {
            if (GroupKeysJsonDataIsDirty == false)
            {
                return GroupKeys;
            }

            groupKeysJsonData = await TryGetLoadGroupKeysJsonData();
            if (groupKeysJsonData == null)
            {
                return null;
            }

            GroupKeys = JsonMapper.ToObject<List<string>>(groupKeysJsonData.ToJson());
            return GroupKeys;
        }

        private JsonData ConvertToJsonData(List<string> groupKeys)
        {
            JsonData jsonData = new JsonData();
            foreach (var groupKey in groupKeys)
            {
                jsonData.Add(groupKey);
            }

            return jsonData;
        }

        private void WriteToDisk(JsonData jsonData)
        {
            using (StreamWriter streamWriter = new StreamWriter(DataArchiveConstData.GetArchiveGroupKeysFilePath(_ArchiveSystemName, _archiveID)))
            {
                streamWriter.AutoFlush = false;
                streamWriter.Write(jsonData.ToJson());
                streamWriter.Flush();
            }
        }
        
        private async Task WriteAysncToDisk(JsonData jsonData)
        {
            WriteCancellationTokenSourceMap.TryGetValue(_archiveID,out var tokenSource);
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }

            string filePath = DataArchiveConstData.GetArchiveGroupKeysFilePath(_ArchiveSystemName, _archiveID);
            bool isUse = await IOHelper.FileIsInUse(filePath, 300,100);
            if (isUse)
            {
                Logger.LOG_ERROR($"文件{filePath}长时间被占用，无法写入辅助信息");
                return;
            }
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                tokenSource = new CancellationTokenSource();
                WriteCancellationTokenSourceMap[_archiveID] = tokenSource;
                ReadOnlyMemory<char> readOnlyMemory = new ReadOnlyMemory<char>(jsonData.ToJson().ToCharArray());
                streamWriter.AutoFlush = false;
                await streamWriter.WriteAsync(readOnlyMemory, tokenSource.Token);
                await streamWriter.FlushAsync();
                tokenSource.Dispose();
                WriteCancellationTokenSourceMap.Remove(_archiveID);
            }
        }

        private async Task<JsonData> TryGetLoadGroupKeysJsonData()
        {
            if (GroupKeysJsonDataIsDirty == false)
            {
                return groupKeysJsonData;
            }

            string saveGroupPath = DataArchiveConstData.GetArchiveGroupKeysFilePath(_ArchiveSystemName, _archiveID);
            
            groupKeysJsonData = JsonMapper.ToObject(await LoadGroupKeysFromDisk(saveGroupPath));
            return groupKeysJsonData;
        }

        private async Task<string> LoadGroupKeysFromDisk(string path)
        {
            if (File.Exists(path) == false)
            {
                return "";
            }
            var sb = new StringBuilder();
            using (var sourceStream = new StreamReader(path))
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

        public void DeleteAllGroupKeyFromDisk()
        {
            string path = DataArchiveConstData.GetArchiveGroupKeysFilePath(_ArchiveSystemName, _archiveID);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            GroupKeys = null;
            groupKeysJsonData = null;
        }

        public async void RemoveGroupKey(string groupKey)
        {
            var groupKeys = await GetAllGroupKey();
            groupKeys.Remove(groupKey);
            groupKeysJsonData = ConvertToJsonData(groupKeys);
            WriteToDisk(groupKeysJsonData);
            GroupKeys = groupKeys;
        }
    }
}