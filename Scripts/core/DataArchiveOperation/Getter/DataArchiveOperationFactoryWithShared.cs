//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public class DataArchiveOperationFactoryWithShared
    {
        private Dictionary<int, IDataArchiveOperation> multiArchiveOperationMap;

        public IDataArchiveOperation GetDataArchiveOperation(int archiveID, ArchiveOperationType operationType)
        {
            if (multiArchiveOperationMap == null)
            {
                multiArchiveOperationMap = new Dictionary<int, IDataArchiveOperation>();
            }

            bool isExist = multiArchiveOperationMap.TryGetValue(archiveID, out var iArchiveOperation);
            if (isExist == false)
            {
                iArchiveOperation = DataArchiveOperationFactory.CreateArchiveOperationObject(operationType, archiveID);
                multiArchiveOperationMap.Add(archiveID, iArchiveOperation);
            }

            return iArchiveOperation;
        }
    }
}