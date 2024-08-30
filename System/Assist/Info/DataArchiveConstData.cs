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
        public static string PREFS_KEY_CUR_ARCHIVE { get; private set; } = USER_KEY + "CurArchID";
        public const string Prefix_ArchiveIDKey = "ArchiveID";
        private static string _ArchiveRootDirectoryPath;

        private static string ArchiveRootDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_ArchiveRootDirectoryPath))
                {
                    _ArchiveRootDirectoryPath = Path.Combine(AppPersistentDataPath, "Archive2024");
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

        public static string GetAndCombineDataFilePath(string path, string groupKey)
        {
            string groupFilePath = System.IO.Path.Combine(path, string.Format("{0}.{1}", groupKey, "archive"));
            return groupFilePath;
        }

        public static int DefaultStartArchiveID = 0;

        public static string GetArchiveKey(int archiveID)
        {
            return string.Format("{0}{1}", Prefix_ArchiveIDKey, archiveID);
        }

        public static string GetArchiveDirectoryPath(int archiveID)
        {
            return Path.Combine(UserArchiveDirectoryPath, GetArchiveKey(archiveID));
        }

        public static string GetArchiveSystemInfoDirectoryPath(int archiveID)
        {
            return Path.Combine(UserArchiveDirectoryPath, GetArchiveKey(archiveID), "DataTypeSystemInfo");
        }

        public static string GetArchiveGroupKeysFilePath(int archiveID)
        {
            return Path.Combine(GetArchiveDirectoryPath(archiveID), "groups.key");
        }

        public static string GetGroupKeyInPlayerPrefs(string groupKey)
        {
            return string.Format($"{USER_KEY}_{groupKey}");
        }
    }
}