//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

#if UNITY_EDITOR
using UnityEngine;

namespace FlexiArchiveSystem.Sample
{
	[ExecuteInEditMode]
	public partial class DemoSample
	{
		void OnGUI()
		{
			GUI.Label(new Rect(Screen.width/2 - 65,Screen.height - 90,130,50),"Flexi Archive System");
			GUI.Label(new Rect(Screen.width/2 - 125,Screen.height - 70,280,50),"Copyright (c) 2024 温文. All rights reserved.");
			GUI.Label(new Rect(Screen.width/2 - 95,Screen.height - 50,200,50),"blog: https://www.unitymake.com");
			GUI.Label(new Rect(Screen.width/2 - 88,Screen.height - 30,200,50),"email: yixiangluntan@163.com");
			if (Application.isPlaying == false || archiveManager == null)
			{
				GUI.Label(new Rect(10, 10, 500, 50), "Please click the play button to test.");
				GUI.Label(new Rect(10, 50, 500, 50),
					$"If you want to query data, you can go to ' {Consts.QueryArchiveToolPath} ' .");
				return;
			}

			GUI.Label(new Rect(10, 10, 500, 50),
				$"If you want to query data, you can go to ' {Consts.QueryArchiveToolPath} ' .");

			GUI.Label(new Rect(10, 50, 500, 50),
				$"Current Archive Index : {archiveManager.ArchiveSetting.CurrentArchiveID}  .");

			var groundWidth = 400;
			var groundHeight = 150;
			var screenWidth = Screen.width;
			var screenHeight = Screen.height;
			var groupx = (screenWidth - groundWidth) / 2;
			var groupy = (screenHeight - groundHeight) / 2;
			GUI.BeginGroup(new Rect(groupx, groupy, groundWidth, groundHeight));
			GUI.Box(new Rect(0, 0, groundWidth, groundHeight), "Select");
			if (GUI.Button(new Rect(10, 30, 120, 30), "1.write(1-1)"))
			{
				Demo1_WriteStr();
			}

			if (GUI.Button(new Rect(10, 70, 120, 30), "2.read(1-1)"))
			{
				Demo2_ReadStrFromDisk();
			}

			if (GUI.Button(new Rect(10, 110, 120, 30), "3.write(2-1):num"))
			{
				Demo3_WriteInt();
			}

			if (GUI.Button(new Rect(140, 30, 120, 30), "4.read(2-1):num"))
			{
				Demo4_ReadIntFromDisk();
			}

			if (GUI.Button(new Rect(140, 70, 120, 30), "5.write(2-2):vec2"))
			{
				Demo5_WriteVector2();
			}

			if (GUI.Button(new Rect(140, 110, 120, 30), "6.read(2-2):vec2"))
			{
				Demo6_ReadVector2();
			}

			if (GUI.Button(new Rect(270, 30, 120, 30), "7.save"))
			{
				Demo7_SavePoint();
			}

			if (GUI.Button(new Rect(270, 70, 120, 30), "8.DeleteAll"))
			{
				Demo_DeleteCurrentArchive();
			}

			if (GUI.Button(new Rect(270, 110, 120, 30), "9.Clone Archive"))
			{
				CloneArchive();
			}

			GUI.EndGroup();
		}
	}
}
#endif