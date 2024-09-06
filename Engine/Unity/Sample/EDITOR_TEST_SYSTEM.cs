//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using UnityEngine;
using Logger = FlexiArchiveSystem.Assist.Logger;

namespace FlexiArchiveSystem.Sample
{
	[ExecuteInEditMode]
	public partial class DemoSample
	{
		private string write_str;
		private string read_str;
		private bool isAsync = true;
		private string archiveID;
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

			if (string.IsNullOrEmpty(write_str) == false)
			{
				GUI.Label(new Rect(10, 90, 500, 50),
					$"Write : {write_str}  .");
			}

			if (string.IsNullOrEmpty(read_str) == false)
			{
				if (string.IsNullOrEmpty(write_str) == false)
				{
					GUI.Label(new Rect(10, 90, 500, 50),
						$"Write : {write_str}  .");
					GUI.Label(new Rect(10, 130, 500, 50),
						$"Read : {read_str}  .");
				}
				else
				{
					GUI.Label(new Rect(10, 90, 500, 50),
						$"Read : {read_str}  .");
				}
				
			}
			
			var groundWidth = 400;
			var groundHeight = 150;
			var screenWidth = Screen.width;
			var screenHeight = Screen.height;
			var groupx = (screenWidth - groundWidth) / 2;
			var groupy = (screenHeight - groundHeight) / 2;
			GUI.BeginGroup(new Rect(groupx, groupy, groundWidth, groundHeight));
			GUI.Box(new Rect(0, 0, groundWidth, groundHeight), "Select");
			isAsync = GUI.Toggle(new Rect(groundWidth - 80, 2, 80, 30),isAsync, "IsAsync");
			if (GUI.Button(new Rect(10, 30, 120, 30), "1.write(1-1)"))
			{
				write_str = Demo1_WriteStr();
			}

			if (GUI.Button(new Rect(10, 70, 120, 30), "2.read(1-1)"))
			{
				read_str = Demo2_ReadStrFromDisk();
			}

			if (GUI.Button(new Rect(10, 110, 120, 30), "3.write(2-1):num"))
			{
				write_str = Demo3_WriteInt();
			}

			if (GUI.Button(new Rect(140, 30, 120, 30), "4.read(2-1):num"))
			{
				read_str = Demo4_ReadIntFromDisk();
			}

			if (GUI.Button(new Rect(140, 70, 120, 30), "5.write(2-2):vec2"))
			{
				write_str = Demo5_WriteVector2();
			}

			if (GUI.Button(new Rect(140, 110, 120, 30), "6.read(2-2):vec2"))
			{
				read_str = Demo6_ReadVector2();
			}

			if (GUI.Button(new Rect(270, 30, 120, 30), "7.save"))
			{
				Demo7_SavePoint(isAsync);
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
			GUI.BeginGroup(new Rect((screenWidth - 200) / 2, groupy + groundHeight + 50, 200, 50));
	
			string lastArchiveID = archiveID;
			string curID = archiveManager.ArchiveSetting.CurrentArchiveID.ToString();
			if (lastArchiveID != curID)
			{
				lastArchiveID = curID;
			}
			archiveID = GUI.TextField(new Rect(40, 0, 120, 20), lastArchiveID);
			GUI.Label(new Rect(50, 25, 120, 20), "Switch Archive");
			if (int.TryParse(archiveID, out var value) == false)
			{
				Logger.LOG_ERROR("请输入存档ID数字");
				archiveID = lastArchiveID;
			}

			if (string.Equals(lastArchiveID, archiveID) == false)
			{
				SwitchArchive(int.Parse(archiveID));
			}
			GUI.EndGroup();
		}
	}
}