using System;
using System.Collections.Generic;
using FlexiArchiveSystem.Serialization;

namespace FlexiArchiveSystem
{
    /// <summary>
    /// 注意：虽然我提供了序列化任意对象的方式,但是我不推荐你使用这个序列化方式，
    /// 因为该方式会序列与值无关的所有元信息，其开销是巨大的。
    /// 因此我在这里警告你，最好不要在正式项目中使用它。
    /// 
    /// Note:Although I did provide a way to serialize object,but i don't recommend you use this serialization method
    /// because it serialize all meta information that is not related to the value, which is expensive.
    /// So I'm here to warn you that it's best not to use it in formal projects.
    /// </summary>
    [Obsolete("Note: 我不推荐你使用这个序列化方式，因为该方式会序列与值无关的所有元信息，其开销是巨大的。因此我在这里警告你，最好不要在正式项目中使用它。")]
    public partial class DataTypeObject: AbstractDataType<System.Object>
    {
        public DataTypeObject(string dataStr) : base(dataStr)
        {
        }
        public override bool Equals(System.Object other) => other.Equals(_dataWraper.value);

        public override string Serialize()
        {
            string str = DataTypeSerializeOperation.SerializeToBinary<System.Object>(_ArchiveOperationType, _dataWraper.value);
            DataWraper<string> wraper = new DataWraper<string>();
            wraper.value = str;
            return DataTypeSerializeOperation.Serialize(_ArchiveOperationType,wraper);
        }

        protected override System.Object DeSerialize(string dataStr)
        {
            string str = DataTypeSerializeOperation.DeSerialize<string>(_ArchiveOperationType, dataStr);
            return DataTypeSerializeOperation.DeSerializeByBinary<System.Object>(_ArchiveOperationType, str);
        }
    }

    public class DataStructList<T> : AbstractDataType<List<T>> where T : struct
    {
        public DataStructList(string dataStr) : base(dataStr)
        {
        }
        public override bool Equals(List<T> another)
        {
            bool isSameRef = (another == data);
            if (isSameRef)
            {
                return true;
            }

            if (another == null || data == null)
            {
                return false;
            }
            
            
            if (another.Count != data.Count)
            {
                return false;
            }

            for (int i = 0; i < another.Count; i++)
            {
                if (another[i].Equals(data[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        protected override string ToString(List<T> data)
        {
            string str = null;
            if (data != null)
            {
                str ="{";
                if (data.Count > 0)
                {
                    str += data[0];
                    for (int i = 1; i < data.Count; i++)
                    {
                        str +="," + data[i];
                    }
                }

                str += "}";
            }

            return str;
        }
    }
    
    public class DataStructArray<T> : AbstractDataType<T[]> where T : struct
    {
        public DataStructArray(string dataStr) : base(dataStr)
        {
            
        }
        public override bool Equals(T[] another)
        {
            bool isSameRef = (another == data);
            if (isSameRef)
            {
                return true;
            }

            if (another == null || data == null)
            {
                return false;
            }
            
            
            if (another.Length != data.Length)
            {
                return false;
            }

            for (int i = 0; i < another.Length; i++)
            {
                if (another[i].Equals(data[i]) == false)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        protected override string ToString(T[] data)
        {
            string str = null;
            if (data != null)
            {
                str ="{";
                if (data.Length > 0)
                {
                    str += data[0];
                    for (int i = 1; i < data.Length; i++)
                    {
                        str +="," + data[i];
                    }
                }

                str += "}";
            }

            return str;
        }
    }
}