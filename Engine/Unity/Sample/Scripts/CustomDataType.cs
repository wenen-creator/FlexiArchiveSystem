//-------------------------------------------------
//            Flexi Archive System
// Copyright (c) 2024 温文. All rights reserved.
//       blog: https://www.unitymake.com
//        email: yixiangluntan@163.com
//-------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlexiArchiveSystem.Sample
{
	/// <summary>
	/// 1.create construct method
	/// 
	/// </summary>
	public class CustomDataType : AbstractDataType<CustomData>
	{
		public CustomDataType(string dataStr) : base(dataStr)
		{

		}
	}

	/// <summary>
	/// 2.override "ToString(CustomData)" function if would know detail data
	/// 
	/// </summary>

	public class CustomData
	{
		public string author = "温文";
		public int code = 1;
		public double luckly = 1.0f;

		public override string ToString()
		{
			return "author: " + author +
			       " code: " + code +
			       " luckly: " + luckly;
		}
	}


	/// <summary>
	/// 3.If it is a complex type that litjson can't serialize,
	/// # you can choose to extend ValueWrapper class,
	/// # I wrapped the conversion method for you.
	/// </summary>
	public class CustomDataWrapper : ValueWrapper<CustomDataWrapper, CustomData2>
	{
		public string author = "温文";
		public int code = 1;
		public double luckly = 1.0f;

		public override void ValueToTheWrapper(CustomData2 value)
		{
			//convert
			author = value.author;
			code = value.code;
			luckly = (double)value.luckly; //float -> double
		}

		public override CustomData2 WrapperToValue()
		{
			//new object
			throw new System.NotImplementedException();
		}
	}

	public class CustomData2
	{
		public string author = "温文";
		public int code = 1;
		public float luckly = 1.0f;

		public override string ToString()
		{
			return "author: " + author +
			       " code: " + code +
			       " luckly: " + luckly;
		}
	}
}
