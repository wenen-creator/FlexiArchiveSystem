//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem
{
    public static partial class DataArchiveConstData
    {
        private static string AppPersistentDataPath = Application.persistentDataPath;

        /// <summary>
        /// there are some platform can require special persistent path for save data
        /// example as weixin.
        /// This is part of the SDK business layer, so the system doesn't want to take on any more responsibility.
        /// please check persistent path is corrent on current paltform, if system with problem
        /// </summary>
        /// <param name="path"></param>
        public static void SetPersistentDataPath(string path)
        {
            AppPersistentDataPath = path;
        }
    }
}