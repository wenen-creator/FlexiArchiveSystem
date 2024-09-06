//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlexiArchiveSystem.ArchiveOperation.IO;
using FlexiArchiveSystem.Assist;
using FlexiArchiveSystem.Setting;
using Mono.Data.Sqlite;

namespace FlexiArchiveSystem.ArchiveOperation
{
    internal class SQLDataArchiveOperation : IDataArchiveOperation, ISetDataArchivePath, ICloneDataArchive
    {
        private string _ArchiveSystemName;
        public string ArchiveSystemName => _ArchiveSystemName;
        public bool IsValidation
        {
            get { return IsActive && Directory.Exists(Path); }
        }
        
        
        private bool IsActive;

        private string FilePath;
        public string Path { get; set; }
        private DataArchiveOperationHelper archiveOperationHelper;

        public DataArchiveOperationHelper ArchiveOperationHelper
        {
            get
            {
                if (archiveOperationHelper == null)
                {
                    SetDataArchiveOperationHelper(DataArchiveOperationFactory.GetDataArchiveOperationHelper());
                }

                return archiveOperationHelper;
            }
        }

        private SqliteConnection connection;
        public List<string> AllGroupKeys;
        public Dictionary<string, Dictionary<string, string>> dataMap;
        private int _archiveID;
        public static bool IsUseConnectionPooling = false;

        public SQLDataArchiveOperation(bool isDelayInitializeDB = true)
        {
            if (isDelayInitializeDB == false)
            {
                InitDBConnection();
            }
        }

        public void SetDataArchiveOperationHelper(DataArchiveOperationHelper helper)
        {
            helper.Init(ArchiveSystemName);
            helper.SetArchiveID(_archiveID);
            archiveOperationHelper = helper;
        }

        public void Init(string moudleName,int archiveID)
        {
            _ArchiveSystemName = moudleName;
            SetArchiveID(archiveID);
            dataMap = new Dictionary<string, Dictionary<string, string>>();
            FilePath = GetAndCombineDataFilePath();
            IsActive = true;
        }

        public void SetArchiveID(int archiveID)
        {
            _archiveID = archiveID;
        }

        public void InitDBConnection()
        {
            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }

            if (File.Exists(FilePath) == false)
            {
                SqliteConnection.CreateFile(FilePath);
            }

            string connectionString = string.Format("Data Source={0};Version={1};", FilePath, 3);
            if (IsUseConnectionPooling)
            {
                connectionString += "Pooling=true;";
            }

            connection = new SqliteConnection(connectionString);
            connection.Open();
        }

        public void DataPersistent(string groupKey, string dataKey, string dataStr)
        {
            if (connection == null)
            {
                InitDBConnection();
            }
            

            string queryGroupTableCmd = $"select name from sqlite_master where type='table' and name='{groupKey}'";
            SqliteCommand groupCommand = new SqliteCommand(queryGroupTableCmd, connection);
            var cmdReader = groupCommand.ExecuteReader();
            bool exsitTable = cmdReader.Read();
            if (exsitTable == false)
            {
                //create table
                string createTableCmd = $"create table {groupKey}(dataKey varchar(32) PRIMARY KEY,data varchar(32));";
                SqliteCommand createTableCommand = new SqliteCommand(createTableCmd, connection);
                createTableCommand.ExecuteNonQuery();
            }

            string updateDataCmd = $"INSERT OR REPLACE into {groupKey} (dataKey,data) values ('{dataKey}','{dataStr}')";
            SqliteCommand queryDataCommand = new SqliteCommand(updateDataCmd, connection);
            queryDataCommand.ExecuteNonQuery();

            bool isExist = TryGetData(groupKey, dataKey, out string lastResult);
            if (isExist == false)
            {
                if (dataMap.ContainsKey(groupKey) == false)
                {
                    dataMap.Add(groupKey, new Dictionary<string, string>());
                }
            }

            dataMap[groupKey][dataKey] = dataStr;
            bool hasExistedBefore = string.IsNullOrEmpty(lastResult) == false;
            if (hasExistedBefore == false)
            {
                TryRecordKey(groupKey);
            }
        }

        public void DataPersistent(params DataObject[] dataObjects)
        {
            foreach (var dataObject in dataObjects)
            {
                var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(dataObject.Key);
                string groupKey = keyTuple.Item1;
                string dataKey = keyTuple.Item2;
                DataPersistent(groupKey, dataKey, dataObject.GetSerializeData());
            }
        }

        public async Task DataPersistentAsync(string groupKey, string dataKey, string dataStr, Action complete)
        {
            if (connection == null)
            {
                InitDBConnection();
            }

            string queryGroupTableCmd = $"select name from sqlite_master where type='table' and name='{groupKey}'";
            SqliteCommand groupCommand = new SqliteCommand(queryGroupTableCmd, connection);
            var cmdReader = await (groupCommand.ExecuteReaderAsync());
            bool exsitTable = (await cmdReader.ReadAsync());
            
            if (exsitTable == false)
            {
                //create table
                string createTableCmd = $"create table {groupKey}(dataKey varchar(32) PRIMARY KEY,data varchar(32));";
                SqliteCommand createTableCommand = new SqliteCommand(createTableCmd, connection);
                await createTableCommand.ExecuteNonQueryAsync();
            }

            string updateDataCmd = $"INSERT OR REPLACE into {groupKey} (dataKey,data) values ('{dataKey}','{dataStr}')";
            SqliteCommand queryDataCommand = new SqliteCommand(updateDataCmd, connection);
            await queryDataCommand.ExecuteNonQueryAsync();

            bool isExist = TryGetData(groupKey, dataKey, out string lastResult);
            if (isExist == false)
            {
                if (dataMap.ContainsKey(groupKey) == false)
                {
                    dataMap.Add(groupKey, new Dictionary<string, string>());
                }
            }

            dataMap[groupKey][dataKey] = dataStr;
            bool hasExistedBefore = string.IsNullOrEmpty(lastResult) == false;
            if (hasExistedBefore == false)
            {
                TryRecordKey(groupKey);
            }
            
            complete?.Invoke();
        }

        public async Task DataPersistentAsync(Action complete, params DataObject[] dataObjects)
        {
            IList<Task> writeTasks = new List<Task>();
            foreach (var dataObject in dataObjects)
            {
                var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(dataObject.Key);
                string groupKey = keyTuple.Item1;
                string dataKey = keyTuple.Item2;
                writeTasks.Add( DataPersistentAsync(groupKey, dataKey, dataObject.GetSerializeData(),null));
            }

            await Task.WhenAll(writeTasks);
            complete?.Invoke();
        }

        public string Read(string groupKey, string dataKey)
        {
            if (File.Exists(FilePath) == false)
            {
                return "";
            }

            if (connection == null)
            {
                InitDBConnection();
            }

            bool isExist = TryGetData(groupKey, dataKey, out string result);
            if (isExist)
            {
                return result;
            }

            string queryGroupTableCmd = $"select name from sqlite_master where type='table' and name='{groupKey}'";
            SqliteCommand groupCommand = new SqliteCommand(queryGroupTableCmd, connection);
            var cmdReader = groupCommand.ExecuteReader();
            bool exsitTable = cmdReader.Read();
            if (exsitTable == false)
            {
                return "";
            }

            string queryDataCmd = $"select data from {groupKey} where dataKey = '{dataKey}'";
            SqliteCommand queryDataCommand = new SqliteCommand(queryDataCmd, connection);
            var queryDataReader = queryDataCommand.ExecuteReader(CommandBehavior.SingleResult);
            if (queryDataReader.Read())
            {
                return queryDataReader.GetString("data");
            }

            return "";
        }

        public bool TryGetData(string groupKey, string dataKey, out string dataStr)
        {
            if (dataMap.ContainsKey(groupKey) == false)
            {
                dataStr = "";
                return false;
            }

            return dataMap[groupKey].TryGetValue(dataKey, out dataStr);
        }

        public async Task DeleteAll()
        {
            TryRemoveAllGroupKey();
            if (Directory.Exists(Path) == false)
            {
                return;
            }
            
            if (connection != null)
            {
                try
                {
                    var disposeAsync = connection.DisposeAsync();
                    await disposeAsync;
                    SqliteConnection.ClearPool(connection);
                    connection = null;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            if (Directory.Exists(Path))
            {
                bool isUse = await IOHelper.FileIsInUse(FilePath, 1000, 200);
                if (isUse)
                {
                    Logger.LOG_ERROR($"文件{FilePath}长时间被占用，无法删除存档");
                    return;
                }
                Directory.Delete(Path, true);
            }

            this.Dispose();
        }

        public void Delete(string groupKey, string dataKey)
        {
            if (File.Exists(FilePath) == false)
            {
                return;
            }

            if (connection == null)
            {
                InitDBConnection();
            }

            string delDataCmd = $"delete from {groupKey} where dataKey='{dataKey}'";
            SqliteCommand delDataCommand = new SqliteCommand(delDataCmd, connection);
            delDataCommand.ExecuteNonQuery();
            if (dataMap != null)
            {
                if (dataMap.TryGetValue(groupKey, out var value))
                {
                    value.Remove(dataKey);
                }
            }
        }

        public void DeleteGroup(string groupKey)
        {
            if (File.Exists(FilePath) == false)
            {
                return;
            }

            if (connection == null)
            {
                InitDBConnection();
            }

            string delDataCmd = $"delete from {groupKey}";
            SqliteCommand delDataCommand = new SqliteCommand(delDataCmd, connection);
            delDataCommand.ExecuteNonQuery();

            if (dataMap.ContainsKey(groupKey))
            {
                dataMap.Remove(groupKey);
            }
        }

        public void TryRecordKey(string groupKey)
        {
            if (AllGroupKeys != null)
            {
                AllGroupKeys.Add(groupKey);
            }

            ArchiveOperationHelper.RecordKey(_archiveID, groupKey);
        }

        public void TryRemoveAllGroupKey()
        {
            ArchiveOperationHelper.DeleteAllGroupKeyFromDisk();
        }
        
        private async Task<List<string>> LoadAllGroupKeyFromDisk()
        {
            return await ArchiveOperationHelper.GetAllGroupKey();
        }

#pragma warning disable CS1998
        public async Task<IDataArchiveSourceWrapper> GetSource()
        {
            if (AllGroupKeys == null)
            {
                AllGroupKeys = await LoadAllGroupKeyFromDisk();
            }
            SqliteArchiveSourceWrapper wrapper = new SqliteArchiveSourceWrapper();
            wrapper.sourcePath = FilePath;
            return wrapper;
        }
#pragma warning restore CS1998
        
        public async Task CloneTo(IDataArchiveSourceWrapper source)
        {
            var wrapper = source as SqliteArchiveSourceWrapper;

            if (string.IsNullOrEmpty(wrapper.sourcePath) || File.Exists(wrapper.sourcePath) == false)
            {
                return;
            }

            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }
            
            byte[] buffer = new byte[0x1000];
            int readLen;
            bool isUse = await IOHelper.FileIsInUse(wrapper.sourcePath, 1000 ,200);
            if (isUse)
            {
                Logger.LOG_ERROR($"文件{wrapper.sourcePath}长时间被占用，无法克隆存档");
                return;
            }

            using var reader = new FileStream(wrapper.sourcePath, FileMode.Open, 
                FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
            using var writer = new FileStream(FilePath, FileMode.OpenOrCreate, 
                FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            while ((readLen = await reader.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await writer.WriteAsync(buffer, 0, buffer.Length);
            }
            ArchiveOperationHelper.RecordAllGroupKeyWhenClone(_archiveID);
        }

        public async Task CloseIOAsync()
        {
            if (connection != null)
            {
                await connection.DisposeAsync();
            }
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }

            archiveOperationHelper = null;
            connection = null;
            IsActive = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task DisposeAsync()
        {
            if (connection != null)
            {
                await connection.DisposeAsync();
            }

            archiveOperationHelper = null;
            connection = null;
            IsActive = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        

        private string GetAndCombineDataFilePath()
        {
            string databasePath = DataArchiveConstData.GetAndCombineDataFilePath(Path, "database");
            return databasePath;
        }
    }
}