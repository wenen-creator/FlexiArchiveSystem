//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace FlexiArchiveSystem.U3DEditor
{
    public class FlexiArchiveMenuItems
    {
        [MenuItem(Consts.ClearAllArchiveToolPath,false,0)]
        public static void ClearAllArchive()
        {
            var path = DataArchiveConstData.UserArchiveDirectoryPath;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Type IFlexiType = typeof(IFlexiDataArchiveManager);
            Assembly assembly = Assembly.GetAssembly(IFlexiType);
            Type[] types = assembly.GetTypes();

            var archiveMgrTypes= types.Where((t)=> t.BaseType != null && t.BaseType.Name == IFlexiType.Name);
            foreach (var type in archiveMgrTypes)
            {
                // IFlexiDataArchiveManager archiveMgr = (IFlexiDataArchiveManager)type.GetField("instance", BindingFlags.Static|BindingFlags.Public).GetValue(null);
                
                IFlexiDataArchiveManager archiveMgr = Activator.CreateInstance(type) as IFlexiDataArchiveManager;
                archiveMgr.Init();
                if (archiveMgr.ArchiveSetting != null)
                {
                    archiveMgr.ArchiveSetting.DeleteArchiveIDData();
                }
            }

        }
        
       
    }
}