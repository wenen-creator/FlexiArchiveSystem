//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.IO;

namespace FlexiArchiveSystem
{
    public static partial class DataArchiveConstData
    {
        private static string AUTHOR { get; set; } = "温文";
        public static string USER_KEY { get; set; } = "Wenen";
        public const string Prefix_ArchiveIDKey = "ArchiveID";
        private const string ArchiveExtensionName = "bin";
        private const string SaveAllGroupKeyFileName = "group";
        private static string _ArchiveRootDirectoryPath;

        private static string ArchiveRootDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_ArchiveRootDirectoryPath))
                {
                    _ArchiveRootDirectoryPath = Path.Combine(AppPersistentDataPath, "tmp");
                }

                return _ArchiveRootDirectoryPath;
            }
        }

        private static string _UserArchiveDirectoryPath;

        public static string UserArchiveDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_UserArchiveDirectoryPath))
                {
                    _UserArchiveDirectoryPath = Path.Combine(ArchiveRootDirectoryPath, USER_KEY);
                }

                return _UserArchiveDirectoryPath;
            }
        }

        public static string GetUserCertainArchiveSystemDirectoryPath(string MoudleName)
        {
             return Path.Combine(UserArchiveDirectoryPath, MoudleName);
        }

        public static string GetAndCombineDataFilePath(string path, string groupKey)
        {
            string fileName = groupKey;
            if (string.IsNullOrEmpty(ArchiveExtensionName) == false)
            {
                fileName = string.Format("{0}.{1}",fileName, ArchiveExtensionName);
            }
            string groupFilePath = Path.Combine(path, fileName);
            return groupFilePath;
        }

        public static int DefaultStartArchiveID = 0;

        public static string GetArchiveKey(int archiveID)
        {
            return string.Format("{0}{1}", Prefix_ArchiveIDKey, archiveID);
        }

        public static string GetArchiveDirectoryPath(string ModuleName, int archiveID)
        {
            return Path.Combine(GetUserCertainArchiveSystemDirectoryPath(ModuleName), GetArchiveKey(archiveID));
        }

        public static string GetArchiveSystemInfoDirectoryPath(string ModuleName, int archiveID)
        {
            return Path.Combine(GetUserCertainArchiveSystemDirectoryPath(ModuleName), GetArchiveKey(archiveID), "DataTypeSystemInfo");
        }

        public static string GetArchiveGroupKeysFilePath(string ModuleName, int archiveID)
        {
            return Path.Combine(GetArchiveDirectoryPath(ModuleName, archiveID), SaveAllGroupKeyFileName);
        }

        public static string GetGroupKeyInPlayerPrefs(string groupKey)
        {
            return string.Format($"{USER_KEY}_{groupKey}");
        }

        public static string GetPrefsKey_CUR_ARCHIVE(string moduleName)
        {
            return USER_KEY + moduleName +"CurArchID";
        }
    }
}