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
    public class DataVector4 : AbstractDataTypeWrapper<Vector4Wrapper>
    {
        public DataVector4(string dataStr) : base(dataStr)
        {

        }

        public override bool Equals(Vector4Wrapper another)
        {
            return another.Equals(data);
        }
    }

    [System.Serializable]
    public struct Vector4Wrapper
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4Wrapper(Vector4 value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
            w = value.w;
        }
        
        public Vector4Wrapper(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Vector4Wrapper(Vector4 value)
        {
            return new Vector4Wrapper(value);
        }

        public static implicit operator Vector4(Vector4Wrapper wrapper)
        {
            return new Vector4(wrapper.x,wrapper.y,wrapper.z,wrapper.w);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})",  x, y, z, w);
        }
    }
}