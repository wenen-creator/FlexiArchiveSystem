//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem
{
    public class DataVector2 : AbstractDataType<Vector2Wrapper>
    {
        public DataVector2(string dataStr) : base(dataStr)
        {

        }
    }

    public struct Vector2Wrapper
    {
        public double x;
        public double y;

        public Vector2Wrapper(Vector2 value)
        {
            x = value.x;
            y = value.y;
        }

        public static implicit operator Vector2Wrapper(Vector2 value)
        {
            return new Vector2Wrapper(value);
        }

        public static implicit operator Vector2(Vector2Wrapper wrapper)
        {
            return new Vector2((float)wrapper.x, (float)wrapper.y);
        }

        public override string ToString()
        {
            return string.Format("[{0}]: ({1},{2})", "Vector2", x, y);
        }
    }
}
