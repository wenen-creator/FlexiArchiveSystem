//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using LitJson;

namespace FlexiArchiveSystem
{
    public struct DataTypeSystemInfo
    {
        public string systemType; //DataType的System.Type

        public DataTypeSystemInfo(string type)
        {
            this.systemType = type;
        }

        public string Serialize()
        {
            return JsonMapper.ToJson(this);
        }
    }
}