//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

namespace FlexiArchiveSystem
{
    public class DataString : AbstractDataType<string>
    {
        public DataString(string dataStr) : base(dataStr)
        {

        }

        public override bool Equals(string other)
        {
            return string.Equals(other, _dataWraper.value);
        }
    }
}
