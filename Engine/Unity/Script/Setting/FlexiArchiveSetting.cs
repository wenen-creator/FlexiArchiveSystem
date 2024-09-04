﻿//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlexiArchiveSystem.ArchiveOperation;
using UnityEngine;

namespace FlexiArchiveSystem
{
    [CreateAssetMenu(fileName = "DataArchiveSetting", menuName = "Flexi Archive System/DataArchiveSetting")]
    public partial class FlexiArchiveSetting : ScriptableObject, IArchiveSetting
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

        public int CurrentArchiveID { get; private set; }
        public List<int> AllArchiveID { get; private set; }
        public IDataArchiveOperation DataArchiveOperation { get; set; }
        public DataSystemInfoArchiveOperation DataTypeSystemInfoOperation { get; set; }
        public string Name => name;
        
        public void Init()
        {
            CurrentArchiveID = LoadCurrentArchiveIDFromDisk();
            CreateOrRebuildArchiveOperation();
        }

        private int LoadCurrentArchiveIDFromDisk()
        {
            return PlayerPrefs.GetInt(DataArchiveConstData.PREFS_KEY_CUR_ARCHIVE,
                DataArchiveConstData.DefaultStartArchiveID);
        }

        public int GetNextArchiveID()
        {
            return CurrentArchiveID + 1;
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
                PlayerPrefs.SetInt(DataArchiveConstData.PREFS_KEY_CUR_ARCHIVE, CurrentArchiveID);
            }

        }

        public void RefreshArchiveOperation()
        {
            DataArchiveOperation.Dispose();
            DataArchiveOperation.Init(CurrentArchiveID);
            RefreshArchiveSystemInfoOperation();
        }

        public void SwitchArchive(int archiveID)
        {
            SetArchiveID(archiveID);
            RefreshArchiveOperation();
        }

        [Conditional("UNITY_EDITOR")]
        public void RefreshArchiveSystemInfoOperation()
        {
            DataTypeSystemInfoOperation.Dispose();
            DataTypeSystemInfoOperation.Init(CurrentArchiveID);
        }

        public void CreateOrRebuildArchiveOperation()
        {
            DataArchiveOperation =
                DataArchiveOperationFactory.CreateArchiveOperationObject(archiveOperationType, CurrentArchiveID);
            DataArchiveOperation.Init(CurrentArchiveID);
            RebuildArchiveSystemInfoOperationInEditor();
        }

        [Conditional("UNITY_EDITOR")]
        private void RebuildArchiveSystemInfoOperationInEditor()
        {
            DataTypeSystemInfoOperation =
                DataArchiveOperationFactory.CreateArchiveSystemInfoOperationObject(archiveOperationType,
                    CurrentArchiveID);
            DataTypeSystemInfoOperation.Init(CurrentArchiveID);
        }

        public List<int> GetAllArchiveID()
        {
            if (AllArchiveID == null)
            {
                AllArchiveID = GetAllArchiveIDFromDisk();
            }

            return AllArchiveID;
        }

        private void RecordCurArchiveIDIntoMemory(int id)
        {
            if (AllArchiveID != null && AllArchiveID.IndexOf(id) == -1)
            {
                AllArchiveID.Add(id);
            }
        }

        public void ClearAllArchiveIDCacheInMemory()
        {
            AllArchiveID = null;
        }

        private List<int> GetAllArchiveIDFromDisk()
        {
            List<int> allArchiveID = null;
            switch (archiveOperationType)
            {
                case ArchiveOperationType.FileMode:
                case ArchiveOperationType.Sqlite:
                    string archiveRootDirectoryPath = DataArchiveConstData.UserArchiveDirectoryPath;
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
    }
}