//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using UnityEditor;

namespace FlexiArchiveSystem.U3DEditor
{
    public class CompileListener : AssetPostprocessor
    {
        private static Action actionWhenCompile;

        public static void RegisterEvent(Action action)
        {
            actionWhenCompile += action;
        }

        public static void RemoveEvent(Action action)
        {
            actionWhenCompile -= action;
        }

        static void OnPostprocessAllAssets(string[] importedAssetPaths)
        {
            foreach (var path in importedAssetPaths)
            {
                if (path.EndsWith(".cs"))
                {
                    actionWhenCompile?.Invoke();
                    return;
                }
            }
        }
    }
}