//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine.Android;

namespace FlexiArchiveSystem
{
	public class DeviceAccess
	{
		public static void ApplyAccess()
		{
#if UNITY_ANDROID
			if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
			{
				Permission.RequestUserPermission(Permission.ExternalStorageRead);
			}
     
			if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
			{
				Permission.RequestUserPermission(Permission.ExternalStorageWrite);
			}
#endif
		}
	}
}

