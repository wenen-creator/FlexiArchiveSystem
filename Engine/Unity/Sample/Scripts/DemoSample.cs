//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.IO;
using UnityEngine;

namespace FlexiArchiveSystem.Sample
{
	public partial class DemoSample : MonoBehaviour
	{
		private IFlexiDataArchiveManager archiveManager;

		/// <summary>
		/// Launch Flexi-Archive System;
		/// </summary>
		private void Awake()
		{
			if (Application.isPlaying == false)
			{
				return;
			}

			DataArchiveConstData.USER_KEY = "Wenen";
			DataManagerSample.instance.Init();
			archiveManager = DataManagerSample.instance;
		}

		private string Demo1_WriteStr()
		{
			DataObject dataObject = archiveManager.GetDataObject("group1", "key1");
			DataString dataString = dataObject.GetData<DataString>();
			dataString.Write(char.ConvertFromUtf32(Random.Range(65, 123)));
			Debug.Log(string.Format($"[{Path.Combine("group1", "key1")}]: write a str ：{dataString.data}"));
			return dataString.data;
		}

		private string Demo2_ReadStrFromDisk()
		{
			DataObject dataObject = archiveManager.GetDataObject("group1", "key1");
			DataString dataString = dataObject.GetData<DataString>();
			string str = dataString.DiskData;
			Debug.Log(string.Format($"[{Path.Combine("group1", "key1")}]: read a str from disk: {str}"));
			return str;
		}

		private string Demo3_WriteInt()
		{
			DataObject dataObject = archiveManager.GetDataObject("group2", "num1");
			DataInteger dataInteger = dataObject.GetData<DataInteger>();
			dataInteger.Write(Random.Range(0, 5));
			Debug.Log(string.Format(
				$"[{Path.Combine("group2", "num1")}]: write a number of type int {dataInteger.data}"));
			return dataInteger.data.ToString();
		}

		private string Demo4_ReadIntFromDisk()
		{
			DataObject dataObject = archiveManager.GetDataObject("group2", "num1");
			DataInteger dataInteger = dataObject.GetData<DataInteger>();
			Debug.Log(string.Format(
				$"[{Path.Combine("group2", "num1")}]: read a int from disk  : {dataInteger.DiskData}"));
			return dataInteger.DiskData.ToString();
		}

		private string Demo5_WriteVector2()
		{
			DataObject dataObject = archiveManager.GetDataObject("group2", "key2");
			DataVector2 dataVector2 = dataObject.GetData<DataVector2>();
			dataVector2.Write(new Vector2(Random.Range(0, 100), Random.Range(0, 100)));
			Debug.Log(string.Format($"[{Path.Combine("group2", "key2")}]: write a vector2 ：{dataVector2.data}"));
			return dataVector2.data.ToString();
		}

		private string Demo6_ReadVector2()
		{
			DataObject dataObject = archiveManager.GetDataObject("group2", "key2");
			DataVector2 dataVector2 = dataObject.GetData<DataVector2>();
			Debug.Log(string.Format(
				$"[{Path.Combine("group2", "key2")}]: read a vector2 from disk：{dataVector2.DiskData}"));
			return dataVector2.DiskData.ToString();
		}

		private void Demo7_SavePoint(bool isAsync)
		{
			Debug.Log(string.Format($"save archive"));
			if (isAsync)
			{
				archiveManager.SaveAsync(() => { Debug.Log("async save successfully");});
			}
			else
			{
				archiveManager.Save();
				Debug.Log("save successfully");
			}
		}

		private void Demo_DeleteCurrentArchive()
		{
			archiveManager.DeleteAll();
			Debug.Log(string.Format($"Delete Current All Data"));
		}

		private void CloneArchive()
		{
			archiveManager.InstantiateNewArchive();
			Debug.Log(string.Format($"Clone New Archive From Current Archive "));
		}
	}
}
