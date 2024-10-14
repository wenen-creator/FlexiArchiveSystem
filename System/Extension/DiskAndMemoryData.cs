//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

namespace FlexiArchiveSystem
{
    public struct DiskAndMemoryData<T>
    {
        public T data;
        public T diskData;

        public DiskAndMemoryData(T data, T diskData)
        {
            this.data = data;
            this.diskData = diskData;
        }
    }
}