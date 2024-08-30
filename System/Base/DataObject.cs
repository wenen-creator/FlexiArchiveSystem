//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
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
         return ArchiveOperation.Read(_Key);
      }

      public void InjectArchiveSetting(IArchiveSetting setting)
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

      private void OnDataPersistent(string dataStr, DataTypeSystemInfo dataTypeSystemInfo)
      {
         isDirty = false;
         ArchiveOperation.DataPersistent(_Key, dataStr);
         _ArchiveSetting.DataTypeSystemInfoOperation.ToSaveDataTypeSystemInfo(_Key, dataTypeSystemInfo);
         OnPersistentHandler?.Invoke(_Key);
      }

      public void Save()
      {
         if (isDirty)
         {
            _dataType.ToPersistent();
         }
      }

      public void Delete()
      {
         ArchiveOperation.Delete(_Key);
         _ArchiveSetting.DataTypeSystemInfoOperation.Delete(_Key);
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