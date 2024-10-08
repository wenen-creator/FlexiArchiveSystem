﻿//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;
using FlexiArchiveSystem.ArchiveOperation;

namespace FlexiArchiveSystem.Setting
{
    /// <summary>
    /// Please go to Unity-FlexiArchiveSetting what is core code;
    /// </summary>
    public partial class FlexiArchiveSetting
    {
        
    }

    public interface IArchiveSetting
    {
        public ArchiveOperationType ArchiveOperationMode { get; }
        public bool IsLog { get; }
        public abstract string ModuleName { get; }
        public bool IsAllowSaveDataSystemInfoInPlayerDevice { get; }
        public int CurrentArchiveID { get; }
        public List<int> AllArchiveID { get; }
        public IDataArchiveOperation DataArchiveOperation { get; set; }
        public DataSystemInfoArchiveOperation DataTypeSystemInfoOperation { get; set; }
        public void Init();
        public int GetNextArchiveID();
        public void SetArchiveID(int val, bool isUpdateToDisk = true);
        public void RefreshArchiveOperation();
        public void SwitchArchive(int archiveID);
        public void CreateOrRebuildArchiveOperation();
        public List<int> GetAllArchiveID();
        public void ClearAllArchiveIDCacheInMemory();
    }
}