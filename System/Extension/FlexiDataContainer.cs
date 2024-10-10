//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.DataType.Base;

namespace FlexiArchiveSystem.Extension
{
    public interface IFlexiWriteOrReadArchiveOperation
    {
        public IFlexiDataArchiveManager DataArchiveManager { get; }

        public DiskAndMemoryData<TValue> GetValue<TDataType, TValue>(string GroupKey, string SubKey)
            where TDataType : AbstractDataTypeWrapper<TValue>;

        public TValue GetDiskValue<TDataType, TValue>(string GroupKey, string SubKey)
            where TDataType : AbstractDataTypeWrapper<TValue>;

        public TValue GetMemoryValue<TDataType, TValue>(string GroupKey, string SubKey)
            where TDataType : AbstractDataTypeWrapper<TValue>;

        public void ModifyValue<TDataType, TValue>(string GroupKey, string SubKey, TValue value)
            where TDataType : AbstractDataTypeWrapper<TValue>;
    }
    public abstract class FlexiDataContainer : IFlexiWriteOrReadArchiveOperation
    {
        public virtual IFlexiDataArchiveManager DataArchiveManager{ get; protected set; }
        
        public void SetDataArchiveManager(IFlexiDataArchiveManager dataArchiveManager)
        {
            DataArchiveManager = dataArchiveManager;
        }

        public DiskAndMemoryData<TValue> GetValue<TDataType,TValue>(string GroupKey, string SubKey) where TDataType: AbstractDataTypeWrapper<TValue>
        {
            return DataArchiveManager.GetValue<TDataType,TValue>(GroupKey, SubKey);
        }
        
        public TValue GetDiskValue<TDataType,TValue>(string GroupKey, string SubKey) where TDataType: AbstractDataTypeWrapper<TValue>
        {
            return DataArchiveManager.GetDiskValue<TDataType,TValue>(GroupKey, SubKey);
        }
        
        public TValue GetMemoryValue<TDataType,TValue>(string GroupKey, string SubKey) where TDataType: AbstractDataTypeWrapper<TValue>
        {
            return DataArchiveManager.GetMemoryValue<TDataType,TValue>(GroupKey, SubKey);
        }

        public void ModifyValue<TDataType,TValue>(string GroupKey, string SubKey, TValue value) where TDataType: AbstractDataTypeWrapper<TValue>
        {
            DataArchiveManager.ModifyValue<TDataType, TValue>(GroupKey, SubKey, value);
        }
    }
}