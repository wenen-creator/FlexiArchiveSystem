//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//        blog: https://unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem.Assist
{
    internal static partial class Logger
    {
        public static void LOG_ERROR(string log)
        {
            Debug.LogError(log);
        }

        public static void LOG_WARNING(string log)
        {
            Debug.LogWarning(log);
        }

        public static void LOG(string log)
        {
            Debug.Log(log);
        }
    }
}