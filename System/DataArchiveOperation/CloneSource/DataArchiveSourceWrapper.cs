//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;
using LitJson;

namespace FlexiArchiveSystem.ArchiveOperation
{
    public class IDataArchiveSourceWrapper
    {

    }

    public class DictionaryJsonArchiveSourceWrapper : IDataArchiveSourceWrapper
    {
        public Dictionary<string, JsonData> source;
    }

    public class SqliteArchiveSourceWrapper : IDataArchiveSourceWrapper
    {
        public string sourcePath;
    }
}