//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

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

			DeviceAccess.ApplyAccess();//高版本的安卓需要向设备申请权限，才能进行读写。

			DataArchiveConstData.USER_KEY = "Wenen";
			DataManagerSample.instance.Init();
			archiveManager = DataManagerSample.instance;
			archiveID = archiveManager.GetLastArchiveID().ToString();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				DataObject dataObject = archiveManager.GetDataObject("group1", "key1");
				DataStructList<float> dataStructList = dataObject.GetData<DataStructList<float>>();
				List<float> list = new List<float>();
				list.Add(Random.Range(0, 5));
				list.Add(Random.Range(0, 5));
				list.Add(Random.Range(0, 5));
				list.Add(Random.Range(0, 5));
				dataStructList.Write(list);
				Debug.Log(string.Format($"[{Path.Combine("group1", "key1")}]: write a str ：{dataStructList.data}"));
			}

			if (Input.GetKeyDown(KeyCode.B))
			{
				DataObject dataObject = archiveManager.GetDataObject("group1", "key1");
				DataStructList<float> dataStructList = dataObject.GetData<DataStructList<float>>();
				Debug.Log(string.Format($"[{Path.Combine("group1", "key1")}]: write a str ：{dataStructList.DiskData}"));
			}
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
			DataObject dataObject = archiveManager.GetDataObject("group2", "key1");
			DataInteger dataInteger = dataObject.GetData<DataInteger>();
			dataInteger.Write(Random.Range(0, 5));
			Debug.Log(string.Format(
				$"[{Path.Combine("group2", "key1")}]: write a number of type int {dataInteger.data}"));
			return dataInteger.data.ToString();
		}

		private string Demo4_ReadIntFromDisk()
		{
			DataObject dataObject = archiveManager.GetDataObject("group2", "key1");
			DataInteger dataInteger = dataObject.GetData<DataInteger>();
			Debug.Log(string.Format(
				$"[{Path.Combine("group2", "key1")}]: read a int from disk  : {dataInteger.DiskData}"));
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

		private string Demo7_WriteObj()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key2");
			DataTypeObject dataTypeObject = dataObject.GetData<DataTypeObject>();
			dataTypeObject.Write(new Vector3Wrapper(new Vector3(6,6,6)));
			Debug.Log(string.Format($"[{Path.Combine("group3", "key2")}]: write a object ：{dataTypeObject.data}"));
			return dataTypeObject.data.ToString();
		}

		private string Demo8_ReadObj()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key2");
			DataTypeObject dataTypeObject = dataObject.GetData<DataTypeObject>();
			string str = "null";
			if (dataTypeObject.DiskData != null)
			{
				Vector3 vector3 = (Vector3Wrapper)dataTypeObject.DiskData;
				str = vector3.ToString();
			}
			Debug.Log(string.Format($"[{Path.Combine("group3", "key2")}]: read a object ：{str}"));
			return str;
		}
		
		private string Demo9_WriteList()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key1");
			DataStructList<float> dataStructList = dataObject.GetData<DataStructList<float>>();
			List<float> list = new List<float>();
			list.Add(Random.Range(0, 5));
			list.Add(Random.Range(0, 5));
			list.Add(Random.Range(0, 5));
			list.Add(Random.Range(0, 5));
			dataStructList.Write(list);
			string log = dataStructList.data.ToString() + " - { ";
			foreach (var ele in dataStructList.data)
			{
				log += ele.ToString();
				log += ",";
			}

			log = log.Remove(log.Length - 1);
			log += " }";
			Debug.Log(($"[{Path.Combine("group3", "key1")}]: write a list ：{log}"));
			return log;
		}
		
		private string Demo10_ReadList()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key1");
			DataStructList<float> dataStructList = dataObject.GetData<DataStructList<float>>();
			string log = "null";
			if (dataStructList.DiskData != null)
			{
				log = dataStructList.DiskData.ToString() + " - { ";
				foreach (var ele in dataStructList.DiskData)
				{
					log += ele.ToString();
					log += ",";
				}
				log = log.Remove(log.Length - 1);
				log += " }";
			}
			
			Debug.Log(($"[{Path.Combine("group3", "key1")}]: write a list ：{log}"));
			return log;
		}
		
		private void Demo_ClearCache()
		{
			archiveManager.ClearMemoryCache();
			Debug.Log(string.Format($"Clear Memory Cache"));
		}

		private void Demo_SavePoint(bool isAsync)
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
		
		private void SwitchArchive(int archiveID)
		{
			archiveManager.SwitchArchiveID(archiveID);
			Debug.Log(string.Format($"Switch Archive {archiveID}"));
		}
	}
}
