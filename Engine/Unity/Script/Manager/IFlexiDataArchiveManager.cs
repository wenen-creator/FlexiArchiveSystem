//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Diagnostics;
using FlexiArchiveSystem.Assist;
using FlexiArchiveSystem.Entry;
using UnityEngine;

namespace FlexiArchiveSystem
{
    /// <summary>
    /// 存档系统
    /// </summary>
    public abstract partial class IFlexiDataArchiveManager : IDisposable 
    {
        public FlexiArchiveSetting ArchiveSetting { get; protected set; }
        
        protected DataArchiveContainer ArchiveContainer;

        public void Init()
        {
            var settingInfo = LoadDataArchiveSettingFromDisk();
            if (settingInfo.ArchiveSetting == null)
            {
                return;
            }
            SetDataArchiveSetting(settingInfo.ArchiveSetting as FlexiArchiveSetting);
            SetDataArchiveSettingName(settingInfo.SettingName);
            InitDataArchiveSetting();
            InitDataArchiveContainer();
            ArchiveManagerRegister.instance.Register(this);
            ListenQuit();
            InitInEditor();
        }

        [Conditional("UNITY_EDITOR")]
        private void InitInEditor()
        {

        }

        private void ListenQuit()
        {
            UnityEngine.Application.quitting += OnApplicationQuit;
        }

        public void SetDataArchiveSetting(FlexiArchiveSetting setting)
        {
#if UNITY_EDITOR
            if (UnityEngine.Application.isPlaying == false)
            {
                ArchiveSetting = UnityEngine.ScriptableObject.CreateInstance<FlexiArchiveSetting>();
                
                ArchiveSetting.ArchiveOperationMode = setting.ArchiveOperationMode;
                ArchiveSetting.hideFlags = UnityEngine.HideFlags.DontSave;
                return;
            }
#endif
            ArchiveSetting = setting;
        }

        private void SetDataArchiveSettingName(string settingName)
        {
            ArchiveSetting.name = settingName;
        }

        public void InitDataArchiveSetting()
        {
            ArchiveSetting.Init();
        }

        protected void InitDataArchiveContainer()
        {
            if (ArchiveContainer != null)
            {
                return;
            }

            ArchiveContainer = new DataArchiveContainer(ArchiveSetting);
        }

        public void InstantiateNewArchive()
        {
            ArchiveContainer.InstantiateNewArchive();
        }

        public DataObject GetDataObject(string groupKey, string dataKey)
        {
            return ArchiveContainer.GetDataObject(groupKey, dataKey);
        }

        public DataGroup GetDataGroup(string groupKey)
        {
            return ArchiveContainer.GetDataGroup(groupKey);
        }

        public void Save() => ArchiveContainer.Save();

        public void SaveAsync(Action complete = null) => ArchiveContainer.SaveAsync(complete);

        public void SaveGroup(string group_key)
        {
            ArchiveContainer.SaveGroup(group_key);
        }
        
        public void SaveGroup(params string[] groups)
        {
            ArchiveContainer.SaveGroup(groups);
        }

        public void Delete(string groupKey, string dataKey)
        {
            ArchiveContainer.Delete(groupKey, dataKey);
        }

        public void DeleteAll()
        {
            ArchiveContainer.DeleteAll();
            ArchiveSetting.ClearAllArchiveIDCacheInMemory();
        }

        public void ClearMemoryCache() => ArchiveContainer.ClearMemoryCache();

        public virtual void Dispose()
        {
            if (ArchiveSetting != null)
            {
#if !UNITY_EDITOR
            UnityEngine.ScriptableObject.DestroyImmediate(ArchiveSetting);
#endif
                ArchiveSetting = null;
            }
        }

        private void OnApplicationQuit()
        {
            if (Application.isPlaying == false)
            {
                UnityEngine.Application.quitting -= OnApplicationQuit;
            }

            if (ArchiveSetting != null)
            {
                ArchiveSetting.DataArchiveOperation?.Dispose();
                ArchiveSetting.DataTypeSystemInfoOperation?.Dispose();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}