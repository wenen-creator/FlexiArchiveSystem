using System;

namespace FlexiArchiveSystem.Extension
{
    public abstract class FlexiDataContainer
    {
        public IFlexiDataArchiveManager DataArchiveManager; 
        public FlexiDataContainer(IFlexiDataArchiveManager dataArchiveManager)
        {
            DataArchiveManager = dataArchiveManager;
        }

        public void ModifyValue<T>(string GroupKey, string SubKey, T value)
        {
            DataObject dataObject = DataArchiveManager.GetDataObject(GroupKey, SubKey);
            Type abstractDataType = DataTypeBinder.GetByValueType<T>();
            IDataType dataString = dataObject.GetData(abstractDataType);
        }
    }
}