//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using LitJson;

namespace FlexiArchiveSystem.Serialization
{
    public class DataTypeSerializeOperation
    {
        public static string Serialize<T>(ArchiveOperationType archiveOperationType, T dataWrapper)
        {
            return JsonMapper.ToJson(dataWrapper);
        }

        public static T DeSerialize<T>(ArchiveOperationType archiveOperationType, string dataStr)
        {
            return JsonMapper.ToObject<AbstractDataType<T>.DataWraper<T>>(dataStr).value;
        }
    }
}