//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using FlexiArchiveSystem.Serialization;

namespace FlexiArchiveSystem
{
    public abstract partial class AbstractDataType<T> : IDataType, IEquatable<T>
    {
        [Serializable]
        public class DataWraper<TData> 
        {
            public TData value;
        }

        protected DataWraper<T> _dataWraper = new DataWraper<T>();
        public T data => _dataWraper.value;
        private T diskData;
        public T DiskData => diskData;
        public event Action OnDirtyHandler;
        private DataTypeSystemInfo _systemInfo;
        protected ArchiveOperationType _ArchiveOperationType;
        DataTypeSystemInfo IDataType.SystemInfo
        {
            get => _systemInfo;
        }
        
        

        public AbstractDataType(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr) == false)
            {
                _dataWraper.value = DeSerialize(dataStr);
                diskData = data;
            }

            _systemInfo = new DataTypeSystemInfo(this.GetType().ToString());
            DataTypeBinder.Register(this.GetType(),typeof(T));
        }

        public void InjectArchiveOperationType(ArchiveOperationType archiveOperationType)
        {
            _ArchiveOperationType = archiveOperationType;
        }

        public void Refresh()
        {
            diskData = data;
        }

        public virtual string Serialize()
        {
            return DataTypeSerializeOperation.Serialize(_ArchiveOperationType, _dataWraper);
        }

        protected virtual T DeSerialize(string dataStr)
        {
            return DataTypeSerializeOperation.DeSerialize<T>(_ArchiveOperationType, dataStr);
        }

        public void Write(T data)
        {
            if (Equals(data) == false)
            {
                _dataWraper.value = data;
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
            return ToString(diskData);
        }
    }
}