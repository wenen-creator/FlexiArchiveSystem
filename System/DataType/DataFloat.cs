//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.DataType.Base;

namespace FlexiArchiveSystem
{
    public class DataFloat : AbstractDataTypeWrapper<float>
    {
        public DataFloat(string dataStr) : base(dataStr)
        {
        }

        public override bool Equals(float another)
        {
            return another.Equals(data);
        }
    }
}