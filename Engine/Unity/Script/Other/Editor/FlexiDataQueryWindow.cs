//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FlexiArchiveSystem.Assist;
using FlexiArchiveSystem.Setting;
using UnityEditor;
using UnityEngine;
using Logger = FlexiArchiveSystem.Assist.Logger;
using Object = UnityEngine.Object;

namespace FlexiArchiveSystem.U3DEditor
{
    public class FlexiDataQueryWindow : EditorWindow
    {
        private string _GroupKey;
        private string _SubKey;
        private static FlexiDataQueryWindow _window;

        private IFlexiDataArchiveManager DataArchiveManager;
        private FlexiArchiveSetting DataArchiveSetting;
        public Dictionary<string, ResultWrapper> keyValuePairs = new Dictionary<string, ResultWrapper>();
        public Queue<string> removeResultQueue = new Queue<string>();
        private bool isQueryError;
        private bool isNonArchiveError;

        public class ResultWrapper
        {
            public string result;
            public string diskResult;
            public string resultType;
        }

        public List<int> AllArchiveID;
        public int selectArchiveID;
        public Vector2 scrollPos;
        bool _foldoutValue;
        public Object field_archiveSetting;

        [MenuItem(Consts.QueryArchiveToolPath, false ,-100)]
        public static void ShowWindow()
        {
            _window = EditorWindow.GetWindow(typeof(FlexiDataQueryWindow),false,"Flexi-ArchiveMonitor") as FlexiDataQueryWindow;
            _window.minSize = new Vector2(600, 100);
            string key = _window.GetLastQueryKey();
            if (string.IsNullOrEmpty(key) == false)
            {
                var items = DataKeyHandler.GetAndProcessKeyCollection(key);
                _window.UpdateKeyField(items.Item1, items.Item2);
            }

            EditorApplication.playModeStateChanged += _window.OnPlayModeStateChanged;
            CompileListener.RegisterEvent(_window.WhenCompile);

            if (Application.isPlaying == false)
            {
                InitAllArchiveManager();
            }
            
            if (_window.DataArchiveManager == null)
            {
                string moduleName = _window.GetLastSelectArchiveSettingName();
                if (string.IsNullOrEmpty(moduleName))
                {
                    moduleName = "Default";
                }
                _window.DataArchiveManager = ArchiveManagerRegister.instance.FindByArchiveSetting(moduleName);
            }
        }

        private void WhenCompile()
        {
            StopArticleMgrService();
        }

        public void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Close();
            }
        }

        private void OnDestroy()
        {
            CompileListener.RemoveEvent(WhenCompile);

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            
            StopArticleMgrService();

            if (string.IsNullOrEmpty(_GroupKey) || string.IsNullOrEmpty(_SubKey))
            {
                return;
            }

            RecordLastQueryKey(DataKeyHandler.CombieGroupAndDataKey(_GroupKey, _SubKey));
        }

        private void StopArticleMgrService()
        {
            if (DataArchiveManager == null)
            {
                return;
            }
            
            if (Application.isPlaying == false)
            {
                DataArchiveManager.Dispose();
                var methodInfo =
                    typeof(IFlexiDataArchiveManager).GetMethod("OnApplicationQuit", BindingFlags.NonPublic);
                methodInfo?.Invoke(DataArchiveManager, null);
            }
        }

        void UpdateKeyField(string groupKey, string subKey)
        {
            _GroupKey = groupKey;
            _SubKey = subKey;
        }

        public string GetLastQueryKey()
        {
            return PlayerPrefs.GetString("DataQueryEditor_LastQuery", "");
        }

        public void RecordLastQueryKey(string key)
        {
            PlayerPrefs.SetString("DataQueryEditor_LastQuery", key);
        }
        
        public string GetLastSelectArchiveSettingName()
        {
            return PlayerPrefs.GetString("DataQueryEditor_LastSelectArchiveSettingName", "");
        }

        public void RecordSelectArchiveSettingName(string moduleName)
        {
            PlayerPrefs.SetString("DataQueryEditor_LastSelectArchiveSettingName", moduleName);
        }

        private void OnGUI()
        {
            //input key
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (field_archiveSetting == null)
            {
                EditorGUILayout.HelpBox("请指定存档设置", MessageType.Error, true);
            }
            else
            {
                if (DataArchiveManager == null)
                {
                    EditorGUILayout.HelpBox("未查询到对应的存档系统", MessageType.Error, true);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("系统配置:", GUILayout.Width(60));
            EditorGUI.BeginChangeCheck();
            field_archiveSetting =
                EditorGUILayout.ObjectField(field_archiveSetting, typeof(FlexiArchiveSetting), false);
            bool isChangeSetting = EditorGUI.EndChangeCheck();
            if (field_archiveSetting == null && DataArchiveManager !=null && DataArchiveManager.ArchiveSetting != null)
            {
                field_archiveSetting = DataArchiveManager.ArchiveSetting;
                isChangeSetting = field_archiveSetting != null;
            }
            if (isChangeSetting)
            {
                if (field_archiveSetting != null)
                {
                    var setting = field_archiveSetting as FlexiArchiveSetting;
                    var mgr = ArchiveManagerRegister.instance.FindByArchiveSetting(setting);
                    DataArchiveManager = mgr;
                    if (mgr != null)
                    {
                        DataArchiveSetting = DataArchiveManager.ArchiveSetting;
                        if (string.IsNullOrEmpty(DataArchiveSetting.ModuleName) == false)
                        {
                            field_archiveSetting.name = DataArchiveSetting.ModuleName;
                        }
                        AllArchiveID = DataArchiveSetting.GetAllArchiveID();
                        isNonArchiveError = AllArchiveID == null || AllArchiveID.Count == 0;
                        selectArchiveID = DataArchiveSetting.CurrentArchiveID;
                        RecordSelectArchiveSettingName(DataArchiveSetting.ModuleName);
                    }
                }
            }


            if (isNonArchiveError)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("没有任何存档数据", MessageType.Warning, true);
            }
            else
            {
                EditorGUILayout.Space(1, false);
                GUILayout.Label("选择存档:", GUILayout.Width(60));
                EditorGUI.BeginDisabledGroup(Application.isPlaying || DataArchiveSetting == null);
                if (EditorGUILayout.DropdownButton(new GUIContent(DataArchiveConstData.GetArchiveKey(selectArchiveID)),
                        FocusType.Keyboard,
                        GUILayout.Width(position.width * 0.4f)))
                {
                    if (Application.isPlaying == false)
                    {
                        GenericMenu genericMenu = new GenericMenu();
                        for (int i = 0; i < AllArchiveID.Count; i++)
                        {
                            int index = i;
                            genericMenu.AddItem(new GUIContent(DataArchiveConstData.GetArchiveKey(AllArchiveID[i])),
                                selectArchiveID == AllArchiveID[i],
                                () => { SelectArchiveIDItem(index); });
                            genericMenu.AddSeparator(string.Empty); //分割线
                        }

                        genericMenu.ShowAsContext();
                    }
                }

                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("输入要查询的数据Key:", MessageType.Info, true);
            // GUILayout.Label("",GUILayout.Width(125));
            float halfWidth = _window.position.width / 2;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mainkey: ", GUILayout.Width(60));
            _GroupKey = EditorGUILayout.TextField("", _GroupKey, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Subkey: ", GUILayout.Width(60));
            _SubKey = EditorGUILayout.TextField("", _SubKey, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            bool clickQueryBtn = GUILayout.Button(new GUIContent("查询", "点击查看值"), GUI.skin.button, GUILayout.Height(21));
            if (clickQueryBtn)
            {
                if (DataArchiveManager == null)
                {
                    isQueryError = true;
                }
                else
                {
                    OnClickQueryButton();
                }
            }

            EditorGUILayout.EndHorizontal();
            if (isQueryError)
            {
                EditorGUILayout.HelpBox("查询失败,输入的Key不存在", MessageType.Error, true);
            }

            if (keyValuePairs.Count > 0)
            {
                EditorGUILayout.Space(20);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width),
                    GUILayout.Height(position.height / 2));
                EditorGUILayout.BeginVertical(GUI.skin.scrollView);
                EditorGUILayout.Separator();
                foreach (var pair in keyValuePairs)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("key: ", GUILayout.Width(30));
                    EditorGUILayout.LabelField($"{pair.Key}", GUI.skin.box, GUILayout.Width(position.width * 0.1f),
                        GUILayout.Height(22));

                    EditorGUILayout.LabelField(new GUIContent("Memory: "), GUILayout.Width(50));
                    EditorGUILayout.LabelField(pair.Value.result, GUI.skin.box, GUILayout.Width(position.width * 0.1f),
                        GUILayout.Height(22));

                    EditorGUILayout.LabelField(new GUIContent("Disk: "), GUILayout.Width(40));
                    EditorGUILayout.LabelField(pair.Value.diskResult, GUI.skin.box,
                        GUILayout.Width(position.width * 0.1f), GUILayout.Height(22));

                    EditorGUILayout.LabelField(new GUIContent("Type: "), GUILayout.Width(40));
                    EditorGUILayout.LabelField(pair.Value.resultType, GUI.skin.box,
                        GUILayout.Width(position.width * 0.1f), GUILayout.Height(22));
                    EditorGUILayout.Space(1);
                    if (GUILayout.Button("Detail", GUILayout.ExpandWidth(true)))
                    {
                        ClickShowDetail(pair.Key);
                    }

                    if (GUILayout.Button("Remove", GUILayout.ExpandWidth(true)))
                    {
                        ClickRemoveMonitor(pair.Key);
                    }

                    EditorGUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(10);
                }

                EditorGUILayout.EndVertical();
                GUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
            OnGUIEnd();
            //value
        }

        private void OnGUIEnd()
        {
            RemoveResultInQueue();
        }

        private void OnClickQueryButton()
        {
            isQueryError = false;
            var dataObject = DataArchiveManager.GetDataObject(_GroupKey, _SubKey);
            string fullKey = DataKeyHandler.CombieGroupAndDataKey(_GroupKey, _SubKey);

            try
            {
                ResultWrapper result = QueryResult(dataObject, fullKey);
                SetResult(fullKey, result);
                AddMonitor(dataObject);
            }
            catch (Exception e)
            {
                isQueryError = true;
#if UNITY_EDITOR && EDITOR_DEV_WENEN
                Assist.Logger.LOG_ERROR(e.Message);          
#endif
            }

        }

        private ResultWrapper QueryResult(DataObject dataObject, string fullKey)
        {
            var keyTuple = DataKeyHandler.GetAndProcessKeyCollection(fullKey);
            string groupKey = keyTuple.Item1;
            string dataKey = keyTuple.Item2;
            Type dataTypeSystemType = null;
            try
            {
                dataTypeSystemType = DataArchiveSetting.DataTypeSystemInfoOperation.GetTypeOfDataValue(groupKey, dataKey);
                
#if UNITY_EDITOR && EDITOR_DEV_WENEN
                if (dataTypeSystemType == null)
                {
                    Logger.LOG_ERROR("SystemInfo信息缺失");
                }
#endif
            }
            catch (Exception)
            {
                Logger.LOG_ERROR("获取SystemInfo出错");
            }
            
            Type valueType = dataTypeSystemType.BaseType.GetGenericArguments()[0];
            var methodInfo_GetData = dataObject.GetType().GetMethod(nameof(dataObject.GetData),new Type[]{});
            methodInfo_GetData = methodInfo_GetData.MakeGenericMethod(dataTypeSystemType);

            object dataType = methodInfo_GetData.Invoke(dataObject, null);

            var methodInfo_DiskToStr =
                dataTypeSystemType.GetMethod("DiskDataToString", BindingFlags.NonPublic | BindingFlags.Instance);
            object diskDataStr = methodInfo_DiskToStr.Invoke(dataType, null);
            ResultWrapper resultWrapper = new ResultWrapper();
            resultWrapper.result = dataType.ToString();
            resultWrapper.diskResult = (string)diskDataStr;
            resultWrapper.resultType = valueType.Name;
            return resultWrapper;
        }

        private void RefreshResult(string key)
        {
            var keyCollection = DataKeyHandler.GetAndProcessKeyCollection(key);
            DataObject dataObject = DataArchiveManager.GetDataObject(keyCollection.Item1, keyCollection.Item2);
            ResultWrapper resultWrapper = QueryResult(dataObject, key);
            UpdateResult(key, resultWrapper.result, resultWrapper.diskResult);
        }

        private void SetResult(string key, ResultWrapper result)
        {
            keyValuePairs[key] = result;
        }

        private void UpdateResult(string key, string result, string diskResult)
        {
            keyValuePairs[key].result = result;
            keyValuePairs[key].diskResult = diskResult;
        }

        private bool RemoveResult(string key)
        {
            return keyValuePairs.Remove(key);
        }

        private void RemoveResultInQueue()
        {
            while (removeResultQueue.Count != 0)
            {
                RemoveResult(removeResultQueue.Dequeue());
            }
        }

        private void AddMonitor(DataObject dataObject)
        {
            dataObject.OnDirtyHandler += RefreshResult;
            dataObject.OnPersistentHandler += RefreshResult;
        }

        private void RemoveMonitor(DataObject dataObject)
        {
            dataObject.OnDirtyHandler -= RefreshResult;
            dataObject.OnPersistentHandler -= RefreshResult;
        }

        private void ClickShowDetail(string key)
        {
            var mousePosInWindowRect = Event.current.mousePosition;
            Rect rect = new Rect(mousePosInWindowRect, Vector2.zero);
            PopupWindow.Show(rect, new DataDetailEditorPopupWindow(key, keyValuePairs[key]));
        }

        private void ClickRemoveMonitor(string key)
        {
            var keyCollection = DataKeyHandler.GetAndProcessKeyCollection(key);
            DataObject dataObject = DataArchiveManager.GetDataObject(keyCollection.Item1, keyCollection.Item2);
            RemoveMonitor(dataObject);
            removeResultQueue.Enqueue(key);
        }

        private void SelectArchiveIDItem(int index)
        {
            var id = AllArchiveID[index];
            if (selectArchiveID == id)
            {
                return;
            }

            selectArchiveID = id;
            DataArchiveSetting.SetArchiveID(selectArchiveID, false);
            DataArchiveSetting.CreateOrRebuildArchiveOperation();
            DataArchiveManager.ClearMemoryCache();
            keyValuePairs.Clear();
        }

        private static void InitAllArchiveManager()
        {
            Type IFlexiType = typeof(IFlexiDataArchiveManager);
            Assembly assembly = Assembly.GetAssembly(IFlexiType);
            Type[] types = assembly.GetTypes();

            var archiveMgrTypes= types.Where((t)=> t.BaseType != null && t.BaseType.Name == IFlexiType.Name);
            foreach (var type in archiveMgrTypes)
            {
                // IFlexiDataArchiveManager archiveMgr = (IFlexiDataArchiveManager)type.GetField("instance", BindingFlags.Static|BindingFlags.Public).GetValue(null);
                
                IFlexiDataArchiveManager archiveMgr = Activator.CreateInstance(type) as IFlexiDataArchiveManager;
                archiveMgr.Init();
            }
        }
    }
}
