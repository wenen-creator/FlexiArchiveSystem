//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.DataType.Base;
using UnityEngine;

namespace FlexiArchiveSystem
{
    public class DataVector3 : AbstractDataTypeWrapper<Vector3Wrapper>
    {
        public DataVector3(string dataStr) : base(dataStr)
        {

        }

        public override bool Equals(Vector3Wrapper another)
        {
            return another.Equals(data);
        }
    }
    
    [System.Serializable]
    public struct Vector3Wrapper
    {
        public float x;
        public float y;
        public float z;

        public Vector3Wrapper(Vector3 value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
        
        public Vector3Wrapper(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3Wrapper(Vector3 value)
        {
            return new Vector3Wrapper(value);
        }

        public static implicit operator Vector3(Vector3Wrapper wrapper)
        {
            return new Vector3((float)wrapper.x, (float)wrapper.y,  (float)wrapper.z);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})",  x, y, z);
        }
    }
}
