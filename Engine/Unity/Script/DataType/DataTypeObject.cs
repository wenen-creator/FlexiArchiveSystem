using System;
using UnityEngine;
using Object = System.Object;

namespace FlexiArchiveSystem
{
    public partial class DataTypeObject
    {
       
        public static implicit operator DataTypeObject(Vector2 value)
        {
            DataTypeObject typeObject = new DataTypeObject("");
            typeObject._dataWraper.value = (new Vector2Wrapper(value));
            return typeObject;
        }
        
        public static implicit operator DataTypeObject(Vector3 value)
        {
            DataTypeObject typeObject = new DataTypeObject("");
            typeObject._dataWraper.value = (new Vector3Wrapper(value));
            return typeObject;
        }
        
        public static implicit operator DataTypeObject(Vector4 value)
        {
            DataTypeObject typeObject = new DataTypeObject("");
            typeObject._dataWraper.value = (new Vector4Wrapper(value));
            return typeObject;
        }
    }
}