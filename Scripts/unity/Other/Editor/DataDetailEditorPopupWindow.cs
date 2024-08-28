//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace FlexiArchiveSystem.U3DEditor
{
    public class DataDetailEditorPopupWindow : PopupWindowContent
    {
        public string Key { get; protected set; }

        public FlexiDataQueryWindow.ResultWrapper ResultWrapper { get; protected set; }

        private Vector2 scrollPos;

        public DataDetailEditorPopupWindow(string key, FlexiDataQueryWindow.ResultWrapper result)
        {
            Key = key;
            ResultWrapper = result;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 200);
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key:", GUILayout.Width(50));
            EditorGUILayout.TextField(Key, GUIStyleSetting.Instance.textAreaStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Memory:", GUILayout.Width(50));
            EditorGUILayout.TextArea(ResultWrapper.result, GUIStyleSetting.Instance.textAreaStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Disk:", GUILayout.Width(50));
            EditorGUILayout.TextArea(ResultWrapper.diskResult, GUIStyleSetting.Instance.textAreaStyle,
                GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            ;
        }
    }
}