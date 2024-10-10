//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.DataType.Base;
using FlexiArchiveSystem.Extension;

namespace FlexiArchiveSystem.Sample
{
    public class DataContainerForSave_Sample : FlexiDataContainer
    {
        public string author
        {
            get => GetMemoryValue<DataString, string>("FlexiDataSave","author");
            set => ModifyValue<DataString, string>("FlexiDataSave","author", value);
        }
        
        public int age
        {
            get => GetMemoryValue<DataInteger, int>("FlexiDataSave","age");
            set => ModifyValue<DataInteger, int>("FlexiDataSave","age", value);
        }
        
        public DiskAndMemoryData<string> author_storage
        {
            get => GetValue<DataString,string>("FlexiDataSave","author");
        }
        
        public DiskAndMemoryData<int> age_storage
        {
            get => GetValue<DataInteger, int>("FlexiDataSave","age");
        }
        

        public DataContainerForSave_Sample(IFlexiDataArchiveManager dataArchiveManager)
        {
            SetDataArchiveManager(dataArchiveManager);
        }
    }
    
    public class DataContainerForSave_Sample2 : IFlexiWriteOrReadArchiveOperation
    {
        public IFlexiDataArchiveManager DataArchiveManager => DataManagerSample.instance;
        public string author
        {
            get => GetMemoryValue<DataString, string>("FlexiDataSave","author");
            set => ModifyValue<DataString, string>("FlexiDataSave","author", value);
        }
        
        public int age
        {
            get => GetMemoryValue<DataInteger, int>("FlexiDataSave","age");
            set => ModifyValue<DataInteger, int>("FlexiDataSave","age", value);
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