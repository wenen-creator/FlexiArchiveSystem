//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;

namespace FlexiArchiveSystem.Serialization
{
    public class DataTypeSerializeOperation
    {
        public static string Serialize<T>(ArchiveOperationType archiveOperationType, T data)
        {
            return JsonMapper.ToJson(data);
        }
        
        public static T DeSerialize<T>(ArchiveOperationType archiveOperationType, string dataStr)
        {
            return JsonMapper.ToObject<AbstractDataType<T>.DataWraper<T>>(dataStr).value;
        }
        
        public static string SerializeToBinary<T>(ArchiveOperationType archiveOperationType, T data)
        {
            string str;
            using (MemoryStream stream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream , data);
                byte[] bytes = stream.GetBuffer();;
                str = Convert.ToBase64String(bytes);
            }
            
            return str;
        }

        public static T DeSerializeByBinary<T>(ArchiveOperationType archiveOperationType, string dataStr)
        {
            T result;
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(dataStr)))
            {
                var binaryFormatter = new BinaryFormatter();
                stream.Position = 0;
                object obj = binaryFormatter.Deserialize(stream);
                result = (T)obj;
            }
            
            return result;
        }
    }
}