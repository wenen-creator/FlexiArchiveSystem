//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlexiArchiveSystem.U3DEditor
{
#if EDITOR_DEV_WENEN
    [CreateAssetMenu(menuName = "Flexi Archive System/GUIStyleSetting")]
#endif
    public class GUIStyleSetting : ScriptableObject
    {

        public GUIStyle textAreaStyle;
        private static GUIStyleSetting instance;

        public static GUIStyleSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<GUIStyleSetting>(Path.Combine("Assets", "Flexi Archive",
                        "Editor", "EditorSetting", "GUI Style Setting.asset"));
                }
                return instance;
            }
        }
    }
}