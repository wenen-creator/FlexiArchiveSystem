//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

namespace FlexiArchiveSystem
{
    public class DataBoolean : AbstractDataType<bool>
    {
        public DataBoolean(string dataStr) : base(dataStr)
        {
        }

        public override bool Equals(bool another)
        {
            return another.Equals(data);
        }
    }
}