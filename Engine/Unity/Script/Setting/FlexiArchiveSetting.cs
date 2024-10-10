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
using FlexiArchiveSystem.ArchiveOperation;
using UnityEngine;

namespace FlexiArchiveSystem.Setting
{
    [CreateAssetMenu(fileName = "DataArchiveSetting", menuName = "Flexi Archive System/DataArchiveSetting")]
    public partial class FlexiArchiveSetting : ScriptableObject, IArchiveSetting, IDisposable
    {
        [SerializeField] private ArchiveOperationType archiveOperationType = ArchiveOperationType.Sqlite;

        public ArchiveOperationType ArchiveOperationMode
        {
            get => archiveOperationType;
            set => archiveOperationType = value;
        }

        [SerializeField] private bool isLog = true;
        public bool IsLog => isLog;
        
        [Header("是否要在玩家设备上存储数据的类型信息")]
        [Tooltip("通常是不需要的。在玩家设备上关闭该选项不会影响数据的存档。该信息只是对应数据的辅助信息，仅为了辅助开发环境的检视。")]
        [SerializeField] private bool _AllowSaveDataSystemInfo = false;
        public bool IsAllowSaveDataSystemInfoInPlayerDevice => _AllowSaveDataSystemInfo;

        [Header("存档系统标识")]
        [SerializeField] private string _ModuleName;
        public string ModuleName => _ModuleName;
        public int CurrentArchiveID { get; private set; }

        [NonSerialized] private List<int> _AllArchiveID = null;
        public List<int> AllArchiveID => _AllArchiveID;
        public IDataArchiveOperation DataArchiveOperation { get; set; }
        public DataSystemInfoArchiveOperation DataTypeSystemInfoOperation { get; set; }

        public void Init()
        {
            CurrentArchiveID = LoadCurrentArchiveIDFromDisk();
            CreateOrRebuildArchiveOperation();
        }

        private int LoadCurrentArchiveIDFromDisk()
        {
            return PlayerPrefs.GetInt(DataArchiveConstData.GetPrefsKey_CUR_ARCHIVE(ModuleName),
                DataArchiveConstData.DefaultStartArchiveID);
        }

        public int GetNextArchiveID()
        {
            GetAllArchiveID();
            var nextArchiveID = CurrentArchiveID + 1;
            while (_AllArchiveID.Contains(nextArchiveID))
            {
                nextArchiveID += 1;
            }
            return nextArchiveID;
        }

        public void SetArchiveID(int val, bool isUpdateToDisk = true)
        {
            if (CurrentArchiveID == val)
            {
                return;
            }

            CurrentArchiveID = val;
            RecordCurArchiveIDIntoMemory(CurrentArchiveID);
            if (isUpdateToDisk)
            {
                PlayerPrefs.SetInt(DataArchiveConstData.GetPrefsKey_CUR_ARCHIVE(ModuleName), CurrentArchiveID);
            }
        }

        public void DeleteArchiveIDData()
        {
            PlayerPrefs.DeleteKey(DataArchiveConstData.GetPrefsKey_CUR_ARCHIVE(ModuleName));
        }

        public void RefreshArchiveOperation()
        {
            DataArchiveOperation.Dispose();
            DataArchiveOperation.Init(ModuleName, CurrentArchiveID);
            DataArchiveOperation.SetDataArchiveOperationHelper(DataArchiveOperation.ArchiveOperationHelper);
            RefreshArchiveSystemInfoOperation();
        }

        public void SwitchArchive(int archiveID)
        {
            // if (CurrentArchiveID != archiveID) { }
            SetArchiveID(archiveID);
            CreateOrRebuildArchiveOperation();   
        }
        
        public void RefreshArchiveSystemInfoOperation()
        {
#if !UNITY_EDITOR
            if (IsAllowSaveDataSystemInfoInPlayerDevice == false)
            {
                return;
            }
#endif
            DataTypeSystemInfoOperation.Dispose();
            DataTypeSystemInfoOperation.Init(ModuleName, CurrentArchiveID);
            DataTypeSystemInfoOperation.SetDataArchiveOperationHelper(DataTypeSystemInfoOperation.ArchiveOperationHelper);
        }

        public void CreateOrRebuildArchiveOperation()
        {
            DataArchiveOperation =
                DataArchiveOperationFactory.CreateArchiveOperationObject(archiveOperationType,ModuleName, CurrentArchiveID);
            DataArchiveOperation.Init(ModuleName,CurrentArchiveID);
            RebuildArchiveSystemInfoOperationInEditor();
        }
        
        private void RebuildArchiveSystemInfoOperationInEditor()
        {
#if !UNITY_EDITOR
            if (IsAllowSaveDataSystemInfoInPlayerDevice == false)
            {
                return;
            }
#endif
            DataTypeSystemInfoOperation =
                DataArchiveOperationFactory.CreateArchiveSystemInfoOperationObject(archiveOperationType,
                    ModuleName, CurrentArchiveID);
            DataTypeSystemInfoOperation.Init(ModuleName, CurrentArchiveID);
        }

        public List<int> GetAllArchiveID()
        {
            if (_AllArchiveID == null)
            {
                _AllArchiveID = GetAllArchiveIDFromDisk();
            }
            
            return _AllArchiveID;
        }

        private void RecordCurArchiveIDIntoMemory(int id)
        {
            if (_AllArchiveID != null && _AllArchiveID.IndexOf(id) == -1)
            {
                _AllArchiveID.Add(id);
            }
        }

        public void ClearAllArchiveIDCacheInMemory()
        {
            _AllArchiveID = null;
        }

        private List<int> GetAllArchiveIDFromDisk()
        {
            List<int> allArchiveID = null;
            switch (archiveOperationType)
            {
                case ArchiveOperationType.FileMode:
                case ArchiveOperationType.Sqlite:
                    string archiveRootDirectoryPath = DataArchiveConstData.GetUserCertainArchiveSystemDirectoryPath(ModuleName);
                    if (Directory.Exists(archiveRootDirectoryPath) == false)
                    {
                        return null;
                    }

                    string[] infos = Directory.GetDirectories(archiveRootDirectoryPath);
                    if (infos != null && infos.Length > 0)
                    {
                        allArchiveID = new List<int>(infos.Length);
                        for (int i = 0; i < infos.Length; i++)
                        {
                            string prefix = DataArchiveConstData.Prefix_ArchiveIDKey;
                            int startIndex = infos[i].LastIndexOf(prefix);
                            int id_Index = startIndex + prefix.Length;
                            int id = int.Parse(infos[i].Substring(id_Index));
                            allArchiveID.Add(id);
                        }
                    }
                    else
                    {
                        allArchiveID = new List<int>(1);
                        allArchiveID[0] = DataArchiveConstData.DefaultStartArchiveID;
                    }
                    break;
                default:
                    throw new Exception("ERROR：当前使用的存档方式，不支持多存档共存机制");
            }

            if (allArchiveID != null && allArchiveID.Count > 1)
            {
                //sort
                allArchiveID.Sort();
            }
            return allArchiveID;
        }

        public void Dispose()
        {
            CurrentArchiveID = 0;
            _AllArchiveID = null;
            DataArchiveOperation = null;
            DataTypeSystemInfoOperation = null;
        }
    }
}