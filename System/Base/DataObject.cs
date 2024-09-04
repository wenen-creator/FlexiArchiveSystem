//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Threading.Tasks;
using FlexiArchiveSystem.ArchiveOperation;

namespace FlexiArchiveSystem
{
   public class DataObject : IDisposable
   {
      private string _Key;

      private string _data;

      public IDataType _dataType;

      public event Action<string> OnDirtyHandler;
      public event Action<string> OnPersistentHandler;
      private IArchiveSetting _ArchiveSetting;

      public bool isDirty { get; protected set; }

      private bool isAsyncSave;
      
      private Action aysncSaveComplete;
      
      private Task aysncSaveTask;

      private string serializeData;

      public string Key
      {
         get => _Key;
         private set => _Key = value;
      }

      private IDataArchiveOperation ArchiveOperation => _ArchiveSetting.DataArchiveOperation;

      public DataObject(string key)
      {
         _Key = key;
      }

      public void Init()
      {
         _data = LoadFromDiskTo();
      }

      private string LoadFromDiskTo()
      {
         var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(_Key);
         string groupKey = keyTuple.Item1;
         string dataKey = keyTuple.Item2;
         return ArchiveOperation.Read(groupKey, dataKey);
      }

      internal void InjectArchiveSetting(IArchiveSetting setting)
      {
         _ArchiveSetting = setting;
      }

      public T GetData<T>() where T : IDataType
      {
         if (_dataType != null)
         {
            return (T)_dataType;
         }

         return CreateDataType<T>();
      }

      private T CreateDataType<T>() where T : IDataType
      {
         var concreteDataType = ConvertTo<T>(_data);
         _dataType = concreteDataType;
         concreteDataType.OnDirtyHandler += SetDirty;
         concreteDataType.OnPersistentHandler += OnDataPersistent;
         concreteDataType.InjectArchiveOperationType(_ArchiveSetting.ArchiveOperationMode);
         return concreteDataType;
      }

      private T ConvertTo<T>(string dataStr) where T : IDataType
      {
         var concreteDataType = (T)Activator.CreateInstance(typeof(T), dataStr);
         return concreteDataType;
      }

      private void SetDirty()
      {
         OnDirtyHandler?.Invoke(_Key);
         isDirty = true;
      }
      
      public void CleanDirty()
      {
         isDirty = false;
      }

      private void OnDataPersistent(string dataStr, DataTypeSystemInfo dataTypeSystemInfo)
      {
         isDirty = false;
         
         var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(_Key);
         string groupKey = keyTuple.Item1;
         string dataKey = keyTuple.Item2;
         
         if (isAsyncSave)
         {
            var temp = aysncSaveComplete;
            aysncSaveTask = ArchiveOperation.DataPersistentAsync(groupKey, dataKey, dataStr, () =>
            {
               temp?.Invoke();
               TryToSaveDataSystemInfo(groupKey, dataKey, dataTypeSystemInfo);
               //TODO : 立刻执行还是等待任务完成
               OnPersistentHandler?.Invoke(_Key);
            });
         }
         else
         {
            ArchiveOperation.DataPersistent(groupKey, dataKey, dataStr);
            TryToSaveDataSystemInfo(groupKey, dataKey, dataTypeSystemInfo);
            OnPersistentHandler?.Invoke(_Key);
         }
         
         
         aysncSaveComplete = null;
      }

      private void TryToSaveDataSystemInfo(string groupKey, string dataKey, DataTypeSystemInfo dataTypeSystemInfo)
      {
#if UNITY_EDITOR
         _ArchiveSetting.DataTypeSystemInfoOperation.ToSaveDataTypeSystemInfo(groupKey, dataKey, dataTypeSystemInfo);
#else
         if (_ArchiveSetting.IsAllowSaveDataSystemInfoInPlayerDevice)
         {
            _ArchiveSetting.DataTypeSystemInfoOperation.ToSaveDataTypeSystemInfo(groupKey, dataKey, dataTypeSystemInfo);
         }
#endif

      }

      public void Save()
      {
         if (isDirty)
         {
            isAsyncSave = false;
            OnDataPersistent(GetSerializeData(), _dataType.SystemInfo);
            _dataType.Refresh();
         }
      }
      
      public async Task SaveAsync(Action complete = null)
      {
         if (isDirty)
         {
            isAsyncSave = true;
            aysncSaveComplete = complete;
            OnDataPersistent(GetSerializeData(), _dataType.SystemInfo);
            if (aysncSaveTask != null)
            {
               await aysncSaveTask;
            }
            _dataType.Refresh();
         }
      }

      internal string GetSerializeData()
      {
         if (isDirty)
         {
            serializeData = _dataType.Serialize();
         }
         return serializeData;
      }

      public void Delete()
      {
         var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(_Key);
         string groupKey = keyTuple.Item1;
         string dataKey = keyTuple.Item2;
         ArchiveOperation.Delete(groupKey, dataKey);
         _ArchiveSetting.DataTypeSystemInfoOperation.Delete(groupKey, dataKey);
         _dataType.Reset();
      }

      public void Dispose()
      {
         _Key = null;
         _data = null;
         _dataType = null;
         OnDirtyHandler = null;
         OnPersistentHandler = null;
      }
   }
}