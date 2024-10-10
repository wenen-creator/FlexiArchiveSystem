//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using FlexiArchiveSystem.DataType.Base;

namespace FlexiArchiveSystem
{
    public class DataDouble : AbstractDataTypeWrapper<double>
    {
        public DataDouble(string dataStr) : base(dataStr)
        {
        }
        
        public override bool Equals(double another)
        {
            return another.Equals(data);
        }
    }
}