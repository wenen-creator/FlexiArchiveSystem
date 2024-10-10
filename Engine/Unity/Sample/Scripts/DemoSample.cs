//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.playcreator.cn
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlexiArchiveSystem.Sample
{
	public partial class DemoSample : MonoBehaviour
	{
		private IFlexiDataArchiveManager archiveManager;
		private DataContainerForSave_Sample dataContainer;

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
			dataContainer = new DataContainerForSave_Sample(archiveManager);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				Demo11_WriteDic();
			}

			if (Input.GetKeyDown(KeyCode.B))
			{
				Demo11_WriteDic();
			}
			
			if (Input.GetKeyDown(KeyCode.J))
			{
				TestPropertyBind_Write("温文",18);
			}
			
			if (Input.GetKeyDown(KeyCode.N))
			{
				TestPropertyBind_Write("Miracle",19);
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
				TestPropertyBind_ReadMemory();
			}
			
			if (Input.GetKeyDown(KeyCode.L))
			{
				TestPropertyBind_ReadDisk();
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
			string str = dataString.diskData;
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
				$"[{Path.Combine("group2", "key1")}]: read a int from disk  : {dataInteger.diskData}"));
			return dataInteger.diskData.ToString();
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
				$"[{Path.Combine("group2", "key2")}]: read a vector2 from disk：{dataVector2.diskData}"));
			return dataVector2.diskData.ToString();
		}

		private string Demo7_WriteObj()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key2");
			DataType_Object DataType_Object = dataObject.GetData<DataType_Object>();
			DataType_Object.Write(new Vector3Wrapper(new Vector3(6,6,6)));
			Debug.Log(string.Format($"[{Path.Combine("group3", "key2")}]: write a object ：{DataType_Object.data}"));
			return DataType_Object.data.ToString();
		}

		private string Demo8_ReadObj()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key2");
			DataType_Object DataType_Object = dataObject.GetData<DataType_Object>();
			string str = "null";
			if (DataType_Object.diskData != null)
			{
				Vector3 vector3 = (Vector3Wrapper)DataType_Object.diskData;
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
			string log = dataStructList.data.ToString() + " - [ ";
			foreach (var ele in dataStructList.data)
			{
				log += ele.ToString();
				log += ",";
			}

			log = log.Remove(log.Length - 1);
			log += " ]";
			Debug.Log(($"[{Path.Combine("group3", "key1")}]: write a list ：{log}"));
			return log;
		}
		
		private string Demo10_ReadList()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key1");
			DataStructList<float> dataStructList = dataObject.GetData<DataStructList<float>>();
			string log = "null";
			if (dataStructList.diskData != null)
			{
				log = dataStructList.diskData.ToString() + " - [ ";
				foreach (var ele in dataStructList.diskData)
				{
					log += ele.ToString();
					log += ",";
				}
				log = log.Remove(log.Length - 1);
				log += " ]";
			}
			
			Debug.Log(($"[{Path.Combine("group3", "key1")}]: write a list ：{log}"));
			return log;
		}
		
		private string Demo11_WriteDic()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key3");
			DataDictionary<Vector3Wrapper> dataDictionary = dataObject.GetData<DataDictionary<Vector3Wrapper>>();
			Dictionary<string, Vector3Wrapper> dictionary = new Dictionary<string, Vector3Wrapper>();
			dictionary.Add(1.ToString(),new Vector3(1,1,1));
			dictionary.Add(2.ToString(),new Vector3(2,2,2));
			dictionary.Add(3.ToString(),new Vector3(3,4,4));
			dataDictionary.Write(dictionary);
			string log = dataDictionary.data.ToString() + " - " + dataDictionary.ToString();
			Debug.Log(($"[{Path.Combine("group3", "key1")}]: write a dictionary ：{log}"));
			return log;
		}
		
		private string Demo12_ReadDic()
		{
			DataObject dataObject = archiveManager.GetDataObject("group3", "key3");
			DataDictionary<Vector3Wrapper> dataDictionary = dataObject.GetData<DataDictionary<Vector3Wrapper>>();
			string log = "null";
			if (dataDictionary.diskData != null)
			{
				log = dataDictionary.diskData.ToString() + " - " + dataDictionary.ToString();
			}
			
			Debug.Log(($"[{Path.Combine("group3", "key1")}]: write a dictionary ：{log}"));
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

		private void TestPropertyBind_Write(string author, int age)
		{
			Debug.Log(string.Format($"[TestPropertyBind_Write] 尝试写入 author: {author} | age:{age}"));
			dataContainer.author = author;
			dataContainer.age = age;
		}
		private void TestPropertyBind_ReadDisk()
		{
			Debug.Log(string.Format($"[TestPropertyBind_Disk] author: {dataContainer.author_storage.diskData} | age:{dataContainer.age_storage.diskData}"));
		}
		private void TestPropertyBind_ReadMemory()
		{
			Debug.Log(string.Format($"[TestPropertyBind_Memory] author: {dataContainer.author} | age:{dataContainer.age}"));
		}
	}
}
