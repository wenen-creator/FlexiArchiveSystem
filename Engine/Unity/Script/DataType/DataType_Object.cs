//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem
{
    public partial class DataType_Object
    {
       
        public static implicit operator DataType_Object(Vector2 value)
        {
            DataType_Object wrapperObject = new DataType_Object("");
            wrapperObject._dataWrapper.value = (new Vector2Wrapper(value));
            return wrapperObject;
        }
        
        public static implicit operator DataType_Object(Vector3 value)
        {
            DataType_Object wrapperObject = new DataType_Object("");
            wrapperObject._dataWrapper.value = (new Vector3Wrapper(value));
            return wrapperObject;
        }
        
        public static implicit operator DataType_Object(Vector4 value)
        {
            DataType_Object wrapperObject = new DataType_Object("");
            wrapperObject._dataWrapper.value = (new Vector4Wrapper(value));
            return wrapperObject;
        }
    }
}