﻿//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;

namespace FlexiArchiveSystem.ArchiveOperation
{
    internal static class DataArchiveOperationFactory
    {
        public static IDataArchiveOperation CreateArchiveOperationObject(ArchiveOperationType operationType,
            string moduleName ,int archiveID)
        {
            IDataArchiveOperation dataArchiveOperation;
            switch (operationType)
            {
                case ArchiveOperationType.FileMode:
                    var fileModeDataArchiveOperation = new FileModeDataArchiveOperation();
                    dataArchiveOperation = fileModeDataArchiveOperation;
                    break;
                case ArchiveOperationType.PlayerPrefs:
                    dataArchiveOperation = new PlayerPrefsDataArchiveOperation();
                    break;
                case ArchiveOperationType.Sqlite:
                    var sqliteDataArchiveOperation = new SQLDataArchiveOperation(true);
                    dataArchiveOperation = sqliteDataArchiveOperation;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
            }

            bool needSetPath = dataArchiveOperation is ISetDataArchivePath;
            if (needSetPath)
            {
                ISetDataArchivePath iSetDataArchivePath = dataArchiveOperation as ISetDataArchivePath;
                iSetDataArchivePath.Path = DataArchiveConstData.GetArchiveDirectoryPath(moduleName, archiveID);
            }

            return dataArchiveOperation;
        }

        public static DataSystemInfoArchiveOperation CreateArchiveSystemInfoOperationObject(
            ArchiveOperationType operationType,string moduleName, int archiveID)
        {
            var DataTypeSystemInfoOperation = new DataSystemInfoArchiveOperation();
            if (DataTypeSystemInfoOperation is ISetDataArchivePath)
            {
                DataTypeSystemInfoOperation.SystemInfoPath =
                    DataArchiveConstData.GetArchiveSystemInfoDirectoryPath(moduleName, archiveID);
            }

            return DataTypeSystemInfoOperation;
        }

        public static DataArchiveOperationHelper GetDataArchiveOperationHelper()
        {
            return new DataArchiveOperationHelper();
        }
    }
}