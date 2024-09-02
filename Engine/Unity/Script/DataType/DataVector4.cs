//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;
namespace FlexiArchiveSystem
{
    public class DataVector4 : AbstractDataType<Vector4Wrapper>
    {
        public DataVector4(string dataStr) : base(dataStr)
        {

        }
    }

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