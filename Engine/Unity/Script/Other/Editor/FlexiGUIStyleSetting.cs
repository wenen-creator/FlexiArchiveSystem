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
    public class FlexiGUIStyleSetting : ScriptableObject
    {

        public GUIStyle textAreaStyle;
        private static FlexiGUIStyleSetting instance;

        public static FlexiGUIStyleSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<FlexiGUIStyleSetting>(Path.Combine(GetAssetToEnginePath(),"Unity",
                        "Editor", "EditorSetting", "GUI Style Setting.asset"));
                }
                return instance;
            }
        }


        static string GetAssetToEnginePath()
        {
            string assetPath = GetRootPath();
            int index = assetPath.IndexOf("Engine");
            return assetPath.Substring(0,index + 6);
        }
        static string GetRootPath()
        {
            string _scriptName = "FlexiGUIStyleSetting";
            
            string[] guidArray = AssetDatabase.FindAssets(_scriptName);
            foreach (string guid in guidArray) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.EndsWith(_scriptName + ".cs")) { 
                    return assetPath;
                }
            }

            return null;
        }
    }
}