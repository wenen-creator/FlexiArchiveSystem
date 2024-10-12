using System;
using System.Collections.Generic;

namespace FlexiArchiveSystem
{
    public class DataTypeBinder
    {
        public static Dictionary<Type, Type> binder;
        
        public static void Register(Type dataType, Type valueType)
        {
            if (binder == null)
            {
                binder = new Dictionary<Type, Type>();
            }

            binder[valueType] = dataType;
        }

        public static Type GetByValueType<T>()
        {
            Type valueType = typeof(T);
            return binder[valueType];
        }
    }
}