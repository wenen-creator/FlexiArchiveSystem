//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using FlexiArchiveSystem.ArchiveOperation;
using LitJson;

namespace FlexiArchiveSystem.ArchiveOperation
{
    /// <summary>
    /// Playerprefs注册表形式的存档方式
    /// notice: 出于性能考虑，因此该方式目前不支持多存档共存。
    /// </summary>
    internal partial class PlayerPrefsDataArchiveOperation : IDataArchiveOperation
    {
        public bool IsValidation
        {
            get { return IsActive; }
        }
        
        private bool IsActive = false;
        
        private Dictionary<string, JsonData> groupDataMap = new Dictionary<string, JsonData>();

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
        
        public List<string> AllGroupKeys;

        public void SetDataArchiveOperationHelper(DataArchiveOperationHelper helper)
        {
            helper.SetArchiveID(_archiveID);
            archiveOperationHelper = helper;
        }

        private int _archiveID;

        public async void Init(int archiveID)
        {
            SetArchiveID(archiveID);
            AllGroupKeys = await LoadAllGroupKeyFromDisk();
            IsActive = true;
        }

        public void SetArchiveID(int archiveID)
        {
            _archiveID = archiveID;
        }

        public void DataPersistent(string groupKey, string dataKey, string dataStr)
        {

            bool hasExistedBefore =TryGetJsonData(groupKey, out JsonData jsonData);
            bool isRewriteGroupKeys = hasExistedBefore == false;
            jsonData[dataKey] = JsonMapper.ToObject(dataStr);
            PlayerPrefs.SetString(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey), jsonData.ToJson());
            if (isRewriteGroupKeys)
            {
                TryRecordKey(groupKey, dataKey);   
            }
        }

        public void DataPersistent(params DataObject[] dataObjects)
        {
            throw new NotImplementedException();
        }

        public async Task DataPersistentAsync(string groupKey, string dataKey, string dataStr, Action complete)
        {
            throw new NotImplementedException("PlayerPrefs does not support asynchronous saving");
        }

        public async Task DataPersistentAsync(Action complete, params DataObject[] dataObjects)
        {
            throw new NotImplementedException("PlayerPrefs does not support asynchronous saving");
        }

        public string Read(string groupKey, string dataKey)
        {
            bool isGet = TryGetJsonData(groupKey, out JsonData jsonData);

            if (isGet == false)
            {
                return "";
            }

            string jsonResult;
            try
            {
                JsonData concreteData_JsonData = jsonData[dataKey];
                jsonResult = concreteData_JsonData == null ||
                             concreteData_JsonData["value"] == null
                    ? ""
                    : concreteData_JsonData.ToJson();

            }
            catch (KeyNotFoundException)
            {
                jsonResult = "";
            }

            return jsonResult;
        }

#pragma warning disable CS1998
        public async Task DeleteAll()
        {
            if (AllGroupKeys == null)
            {
                AllGroupKeys = await LoadAllGroupKeyFromDisk();
            }

            if (AllGroupKeys != null)
            {
                foreach (var groupKey in AllGroupKeys)
                {
                    PlayerPrefs.DeleteKey(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey));
                }
            }
            TryRemoveAllGroupKey();
        }
#pragma warning restore CS1998

        public void Delete(string groupKey, string dataKey)
        {
            bool isGet = TryGetJsonData(groupKey, out JsonData jsonData);
            if (isGet == false)
            {
                return;
            }

            jsonData[dataKey] = null;
            PlayerPrefs.SetString(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey), jsonData.ToJson());
        }

        public void DeleteGroup(string groupKey)
        {
            if (PlayerPrefs.HasKey(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey)) == false)
            {
                return;
            }

            PlayerPrefs.DeleteKey(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey));
            ArchiveOperationHelper.RemoveGroupKey(groupKey);
        }

        public virtual void TryRecordKey(string groupKey, string dataKey)
        {
            if (AllGroupKeys != null)
            {
                AllGroupKeys.Add(groupKey);
            }
            ArchiveOperationHelper.RecordKey(_archiveID, groupKey, dataKey);
        }

        public void TryRemoveAllGroupKey()
        {
            ArchiveOperationHelper.DeleteAllGroupKeyFromDisk();
        }

        private async Task<List<string>> LoadAllGroupKeyFromDisk()
        {
            return await ArchiveOperationHelper.GetAllGroupKey();
        }

        private bool TryGetJsonData(string groupKey, out JsonData jsonData)
        {
            jsonData = null;
            if (groupDataMap.TryGetValue(groupKey, out jsonData) == false)
            {
                string str = LoadFromDisk(groupKey);
                if (string.IsNullOrEmpty(str))
                {
                    jsonData = new JsonData();
                    groupDataMap.Add(groupKey, jsonData);
                    return false;
                }

                jsonData = JsonMapper.ToObject(str);
                groupDataMap.Add(groupKey, jsonData);
            }

            return true;
        }

        private string LoadFromDisk(string groupKey)
        {
            if (PlayerPrefs.HasKey(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey)) == false)
            {
                return "";
            }

            string str = PlayerPrefs.GetString(DataArchiveConstData.GetGroupKeyInPlayerPrefs(groupKey), "");
            return str;
        }

        public void Dispose()
        {
            AllGroupKeys = null;
            groupDataMap = null;
            IsActive = false;
        }
    }
}