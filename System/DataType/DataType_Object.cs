//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using FlexiArchiveSystem.DataType.Base;
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
    public partial class DataType_Object: AbstractDataTypeWrapper<System.Object>
    {
        public DataType_Object(string dataStr) : base(dataStr)
        {
        }
        public override bool Equals(System.Object other) => other.Equals(_dataWrapper.value);

        public override string Serialize()
        {
            string str = DataTypeSerializeOperation.SerializeToBinary<System.Object>(_ArchiveOperationType, _dataWrapper.value);
            DataResultWrapper<string> wrapper = new DataResultWrapper<string>();
            wrapper.value = str;
            return DataTypeSerializeOperation.Serialize(_ArchiveOperationType,wrapper);
        }

        protected override System.Object DeSerialize(string dataStr)
        {
            string str = DataTypeSerializeOperation.DeSerialize<string>(_ArchiveOperationType, dataStr);
            return DataTypeSerializeOperation.DeSerializeByBinary<System.Object>(_ArchiveOperationType, str);
        }
    }

    public class DataStructList<T> : AbstractDataTypeWrapper<List<T>> where T : struct
    {
        public DataStructList(string dataStr) : base(dataStr)
        {
        }
        public override bool Equals(List<T> another)
        {
            return another == data;
        }

        public bool ElementsIsEqual(List<T> another)
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
                str ="[";
                if (data.Count > 0)
                {
                    str += data[0];
                    for (int i = 1; i < data.Count; i++)
                    {
                        str +="," + data[i];
                    }
                }

                str += "]";
            }

            return str;
        }
    }
    
    public class DataStructArray<T> : AbstractDataTypeWrapper<T[]> where T : struct
    {
        public DataStructArray(string dataStr) : base(dataStr)
        {
            
        }
        public override bool Equals(T[] another)
        {
            return another == data;
        }

        public bool ElementsIsEqual(T[]another)
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
                str ="[";
                if (data.Length > 0)
                {
                    str += data[0];
                    for (int i = 1; i < data.Length; i++)
                    {
                        str +="," + data[i];
                    }
                }

                str += "]";
            }

            return str;
        }
    }
    
    public class DataList<T> : AbstractDataTypeWrapper<List<T>> 
    {
        public DataList(string dataStr) : base(dataStr)
        {
        }
        public override bool Equals(List<T> another)
        {
            return another == data;
        }

        public bool ElementsIsEqual(List<T> another)
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
                str ="[";
                if (data.Count > 0)
                {
                    str += data[0].ToString();
                    for (int i = 1; i < data.Count; i++)
                    {
                        str +="," + data[i];
                    }
                }

                str += "]";
            }

            return str;
        }
    }
    
    public class DataArray<T> : AbstractDataTypeWrapper<T[]> 
    {
        public DataArray(string dataStr) : base(dataStr)
        {
            
        }
        public override bool Equals(T[] another)
        {
            return another == data;
        }

        public bool ElementsIsEqual(T[] another)
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
                str ="[";
                if (data.Length > 0)
                {
                    str += data[0].ToString();
                    for (int i = 1; i < data.Length; i++)
                    {
                        str +="," + data[i];
                    }
                }

                str += "]";
            }

            return str;
        }
    }

    internal class DataDictionary<TValue> : AbstractDataTypeWrapper<Dictionary<string, TValue>> 
    {
        public DataDictionary(string dataStr) : base(dataStr)
        {
            
        }
        
        public override bool Equals(Dictionary<string, TValue> another)
        {
            return another.Equals(data);
        }

        protected override string ToString(Dictionary<string, TValue> data)
        {
            string str = null;
            if (data != null)
            {
                str ="{";
                if (data.Count > 0)
                {
                    var keyCollections = data.Keys;
                    foreach (var key in keyCollections)
                    {
                        str += string.Format("'{0}' : {1}, ", key, data[key].ToString());
                    }

                    str = str.Remove(str.Length - 2,2);
                }

                str += "}";
            }

            return str;
        }
    }
    
}