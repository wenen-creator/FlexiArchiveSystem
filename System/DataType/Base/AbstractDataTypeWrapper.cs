//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Reflection;
using FlexiArchiveSystem.Assist;
using FlexiArchiveSystem.Serialization;

namespace FlexiArchiveSystem.DataType.Base
{
    public abstract partial class AbstractDataTypeWrapper<T> : IDataType, IEquatable<T>
    {
        [Serializable]
        public class DataResultWrapper<TData> 
        {
            public TData value;
        }

        protected DataResultWrapper<T> _dataWrapper = new DataResultWrapper<T>();
        public T data => _dataWrapper.value;
        private T _diskData;
        public T diskData => _diskData;
        public event Action OnDirtyHandler;
        private DataTypeSystemInfo _systemInfo;
        DataTypeSystemInfo IDataType.SystemInfo => _systemInfo;
        protected ArchiveOperationType _ArchiveOperationType;
        private MethodInfo _methodInfoOfWriteData;


        public AbstractDataTypeWrapper(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                _dataWrapper.value = DeSerialize(dataStr);
                _diskData = data;
            }

            _systemInfo = new DataTypeSystemInfo(this.GetType().ToString());
            // DataTypeBinder.Register(this.GetType(),typeof(T)); //暂时没用上
        }

        public void InjectArchiveOperationType(ArchiveOperationType archiveOperationType)
        {
            _ArchiveOperationType = archiveOperationType;
        }

        public MethodInfo GetWriteDataMethodInfo()
        {
            if (_methodInfoOfWriteData == null)
            {
                _methodInfoOfWriteData = this.GetType().GetMethod("Write",BindingFlags.Public | BindingFlags.Instance);
            }
            return _methodInfoOfWriteData;
        }

        public void Refresh()
        {
            _diskData = data;
        }

        public virtual string Serialize()
        {
            return DataTypeSerializeOperation.Serialize(_ArchiveOperationType, _dataWrapper);
        }

        protected virtual T DeSerialize(string dataStr)
        {
            return DataTypeSerializeOperation.DeSerialize<T>(_ArchiveOperationType, dataStr);
        }

        public void WriteByGenericObject(object genericValue)
        {
            if (genericValue is T concreteData)
            {
                Write(concreteData);
                return;
            }
            Logger.LOG_ERROR("The data type does not match");
        }
        
        public void Write(T data)
        {
            if (Equals(data) == false)
            {
                _dataWrapper.value = data;
                OnDirtyHandler?.Invoke();
            }
        }

        public abstract bool Equals(T another);

        public void Reset()
        {
            Write(default(T));
        }

        public override string ToString()
        {
            return ToString(data);
        }

        protected virtual string ToString(T data)
        {
            return data.ToString();
        }

        protected virtual string DiskDataToString()
        {
            return ToString(_diskData);
        }
    }
}