//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

namespace FlexiArchiveSystem
{
    public abstract class ValueWrapper<TWrapper, TValue> where TWrapper : ValueWrapper<TWrapper, TValue>, new()
    {
        public abstract TValue WrapperToValue();

        public abstract void ValueToTheWrapper(TValue value);


        /// <summary>
        /// 该操作会将Value包装进引用类型的对象中，会产生额外的GC开销。
        /// 建议降低使用频率
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator ValueWrapper<TWrapper, TValue>(TValue value)
        {
            var wrapper = new TWrapper();
            wrapper.ValueToTheWrapper(value);
            return wrapper;
        }

        public static implicit operator TValue(ValueWrapper<TWrapper, TValue> wrapper)
        {
            return wrapper.WrapperToValue();
        }
    }
}