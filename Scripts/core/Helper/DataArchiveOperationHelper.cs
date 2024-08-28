//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LitJson;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public class DataArchiveOperationHelper
    {
        private List<string> GroupKeys;
        private JsonData groupKeysJsonData;
        private bool GroupKeysJsonDataIsDirty => groupKeysJsonData == null;

        private int _archiveID = -1;

        public void SetArchiveID(int archiveID)
        {
            _archiveID = archiveID;
        }

        [Conditional("UNITY_EDITOR")]
        public void RecordKey(int archiveID, string groupKey, string dataKey)
        {
            UpdateDirtyState(archiveID);
            //记录key
            TryAddGroupKey(groupKey);
        }

        [Conditional("UNITY_EDITOR")]
        public void RecordAllGroupKey(int archiveID, List<string> groupKeys)
        {
            UpdateDirtyState(archiveID);
            //记录key
            GroupKeys = groupKeys;
            groupKeysJsonData = ConvertToJsonData(groupKeys);
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

        public void TryAddGroupKey(string groupKey)
        {
            var groupKeys = GetAllGroupKey();
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
            groupKeysJsonData = ConvertToJsonData(groupKeys);
            WriteToDisk(groupKeysJsonData);
        }

        public List<string> GetAllGroupKey()
        {
            if (GroupKeysJsonDataIsDirty == false)
            {
                return GroupKeys;
            }

            JsonData groupKeysJson = TryGetLoadGroupKeysJsonData();
            if (groupKeysJson == null)
            {
                return null;
            }

            return JsonMapper.ToObject<List<string>>(groupKeysJson.ToJson());
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
            // PlayerPrefs.SetString(, jsonData.ToJson());
            File.WriteAllText(DataArchiveConstData.GetArchiveGroupKeysFilePath(_archiveID), jsonData.ToJson());
        }

        private JsonData TryGetLoadGroupKeysJsonData()
        {
            if (GroupKeysJsonDataIsDirty == false)
            {
                return groupKeysJsonData;
            }

            string saveGroupPath = DataArchiveConstData.GetArchiveGroupKeysFilePath(_archiveID);
            groupKeysJsonData = JsonMapper.ToObject(LoadGroupKeysFromDisk(saveGroupPath));
            return groupKeysJsonData;
        }

        private string LoadGroupKeysFromDisk(string path)
        {
            if (File.Exists(path) == false)
            {
                return "";
            }

            string str = File.ReadAllText(path);
            return str;
        }

        public void DeleteAllGroupKeyFromDisk()
        {
            string path = DataArchiveConstData.GetArchiveGroupKeysFilePath(_archiveID);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            GroupKeys = null;
            groupKeysJsonData = null;
        }

        public void RemoveGroupKey(string groupKey)
        {
            var groupKeys = GetAllGroupKey();
            groupKeys.Remove(groupKey);
            groupKeysJsonData = ConvertToJsonData(groupKeys);
            WriteToDisk(groupKeysJsonData);
            GroupKeys = groupKeys;
        }
    }
}