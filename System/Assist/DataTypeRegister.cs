//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;

namespace FlexiArchiveSystem
{
    public class DataTypeBinder
    {
        //{valueWrapperMeta : dataTypeMeta}
        public static Dictionary<Type, Type> binder;
        
        /// <summary>
        /// bind value and AbstractDataTypeWrapper<T>
        /// </summary>
        /// <param name="dataTypeMeta"> AbstractDataTypeWrapper </param>
        /// <param name="valueTypeMeta">Type of value</param>
        public static void Register(Type dataTypeMeta, Type valueTypeMeta)
        {
            if (binder == null)
            {
                binder = new Dictionary<Type, Type>();
            }
            
            binder[valueTypeMeta] = dataTypeMeta;
        }

        /// <summary>
        /// 需要先注册绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetByValueType<T>()
        {
            Type valueTypeMeta = typeof(T);
            if (binder.TryGetValue(valueTypeMeta, out var dataTypeMeta))
            {
                return dataTypeMeta;
            }
            return null;
        }
    }
}