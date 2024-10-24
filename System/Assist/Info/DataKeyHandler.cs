//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.IO;

namespace FlexiArchiveSystem
{
    public class DataKeyHandler
    {
        public static Tuple<string, string> GetAndProcessKeyCollection(string key)
        {
            //spilt group and data key
            string[] keys = key.Split(Path.DirectorySeparatorChar);
            string groupKey = keys[0];
            string dataKey = keys[1];
            if (string.IsNullOrEmpty(dataKey))
            {
                throw new InvalidDataException("Archiving accident! The Key of the data is invalid.");
            }

            if (string.IsNullOrEmpty(groupKey))
            {
                groupKey = "Global";
            }

            return new Tuple<string, string>(groupKey, dataKey);
        }

        public static string CombieGroupAndDataKey(string groupKey, string dataKey)
        {
            return Path.Combine(groupKey, dataKey);
        }
    }
}