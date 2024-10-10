//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
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
			GUI.Label(new Rect(Screen.width/2 - 95,Screen.height - 50,200,50),"blog: https://www.playcreator.cn");
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
			var groundHeight = 200;
			var screenWidth = Screen.width;
			var screenHeight = Screen.height;
			var groupx = (screenWidth - groundWidth) / 2;
			var groupy = (screenHeight - groundHeight) / 2;
			GUI.BeginGroup(new Rect(groupx, groupy, groundWidth, groundHeight));
			GUI.Box(new Rect(0, 0, groundWidth, groundHeight), "Select");
			isAsync = GUI.Toggle(new Rect(groundWidth - 80, 2, 80, 30),isAsync, "IsAsync");
			List<Tuple<string, Action>> ele = new List<Tuple<string, Action>>();
			ele.Add(new Tuple<string, Action>("1.write(1-1):str", () => { write_str = Demo1_WriteStr(); }));
			ele.Add(new Tuple<string, Action>("2.read(1-1):str", () => { read_str = Demo2_ReadStrFromDisk(); }));
			ele.Add(new Tuple<string, Action>("3.write(2-2):vec2", () => { write_str = Demo5_WriteVector2(); }));
			
			ele.Add(new Tuple<string, Action>("4.read(2-2):vec2", () => { read_str = Demo6_ReadVector2(); }));
			ele.Add(new Tuple<string, Action>("5.write(3-1):list", () => { write_str = Demo9_WriteList(); }));
			ele.Add(new Tuple<string, Action>("6.read(3-1):list", () => { read_str = Demo10_ReadList(); }));
			
			ele.Add(new Tuple<string, Action>("7.write(3-2):obj", () => { write_str = Demo7_WriteObj(); }));
			ele.Add(new Tuple<string, Action>("8.read(3-2):obj", () => { read_str = Demo8_ReadObj(); }));
			ele.Add(new Tuple<string, Action>("9.Clear Cache", () => {  Demo_ClearCache(); }));
			
			ele.Add(new Tuple<string, Action>("10.save", () => { Demo_SavePoint(isAsync); }));
			ele.Add(new Tuple<string, Action>("11.DeleteAll", () => { Demo_DeleteCurrentArchive(); }));
			ele.Add(new Tuple<string, Action>("12.Clone Archive", () => { CloneArchive(); }));

			
			for (int i = 0; i < ele.Count; i++)
			{
				int rowIndex = i / 3;
				int colIndex = i % (ele.Count / 4);
				if (GUI.Button(new Rect(10 + colIndex * 130, 30 + rowIndex * 40, 120, 30), ele[i].Item1))
				{
					ele[i].Item2.Invoke();
				}
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