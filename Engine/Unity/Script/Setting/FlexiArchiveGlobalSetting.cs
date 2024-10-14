//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;

namespace FlexiArchiveSystem.Setting
{
    [CreateAssetMenu(fileName = "GlobalSetting", menuName = "Flexi Archive System/GlobalSetting")]
    public class FlexiArchiveGlobalSetting : ScriptableObject
    {

        private static FlexiArchiveGlobalSetting _instance;

        public static FlexiArchiveGlobalSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<FlexiArchiveGlobalSetting>("GlobalSetting");
                }
                return _instance;
            }
        }
        
        [SerializeField] private int _MaxIOCheckTime = 1000; //ms

        public int MaxIOCheckTime => _MaxIOCheckTime;

        public void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }
    }
}