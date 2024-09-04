# Flexi Archive System  [![license](https://img.shields.io/badge/%20license-LGPL--2.1-brightgreen?link=https%3A%2F%2Fgithub.com%2Fwenen-creator%2FFlexiArchiveSystem%2Fblob%2Fmaster%2FLICENSE)](https://github.com/wenen-creator/FlexiArchiveSystem/blob/master/LICENSE)  [![license](https://img.shields.io/badge/Author-%E6%B8%A9%E6%96%87-blue?color=%2333A1C9)](https://github.com/wenen-creator)

&ensp;&ensp;*Flexi Archive* 是一个专门为 Unity3D 设计的数据存档工具。

&ensp;&ensp;正如 *Flexi Archive* 名字一样，*Flexi Archive* 以 **Flexible** 为设计理念，旨在通过其高度可扩展性、易上手、高性能以及完善的工具等特点，提供一个满足复杂存档需求的灵活而强大的解决方案。

&ensp;&ensp;在 *Flexi Archive System* 中你可以用轻松地几行代码跨平台保存几乎任何东西，同时 *Flexi Archive System* 以其高度可扩展的架构设计，允许你根据实际需求轻松的自定义数据类型和存档策略、数据格式。

&ensp;&ensp;Flexi Archive System 系统采用了高效的存储机制。默认采用按需载入原则，通过合批、异步IO、缓存机制、分组策略、脏标记等大量优化策略，确保用户在进行大量频繁的数据操作时，也能够快速响应。得以轻松应对游戏中复杂需求。


## 文档

&ensp;&ensp;建议你在使用 *Flexi Archive* 前，先打开 **Sample** 案例。

&ensp;&ensp;在 Sample 案例中，你可以快速学习到 *Flexi Archive System* 的核心功能。

![img](https://github.com/wenen-creator/FlexiArchiveSystem/blob/dev/Misc/img/preview01.PNG)

## 系统特点

### 1.支持同一设备下多账号存档共存 

&ensp;&ensp;在用户登录成功后设置 USER_KEY 即可。若你的程序无登录业务，你可以不用设置。
示例：

``` C#
    void Into() { DataArchiveConstData.USER_KEY = "Wenen"; }
```

### 2.支持多存档机制 

&ensp;&ensp;你可以在合适的时机，为当前存档克隆一份新的存档。存档间是隔离的，修改某个存档，不会对其他存档造成影响。

&ensp;&ensp;当然你也可以自由的切换、删除存档。


### 3.支持多种序列化方式 

&ensp;&ensp;支持 File、PlayerPrefs、SQL-DB 异步序列化（存档/读档）方式。你可以根据项目模块需求以及性能考量，自由决定该模块存档系统所使用的序列化方式，默认为 SQL-DB 方式。

&ensp;&ensp;多存档支持：

1. File方式：支持多存档
2. PlayerPrefs方式：出于性能考虑，暂不支持多存档
3. SQL-DB方式：支持多存档

&ensp;&ensp;分析：
1. File方式(JSON)：适用于存档量需求适中的项目，方便用于云存档上传。 
2. PlayerPrefs方式：适用于单个数据量小且每组存档量少的存档，访问较快。如记录下用户某个操作、用户本地所做的设置等。 
3. SQL-DB方式：适用于存档量需求较大的项目，读取开销与压缩比相比File方式在大多情况下要低。

### 4.支持创建多个存档系统 

&ensp;&ensp;你可以根据程序模块的不同，自由的创建多个不同的存档系统。

&ensp;&ensp;方便你根据具体的模块来定制扩展存档系统，也可以选择更适合该模块的序列化存档方式。

### 5.保存点存档 

&ensp;&ensp;你需要在合适的时机，触发存档操作。否则你对数据的修改，只会使 Memory 的数据发生变化。

&ensp;&ensp;值得一提的是，Flexi Archive System 只会对发生变化的数据进行存档。

### 6.分组策略 

&ensp;&ensp;Flexi Archive System 使用 GroupKey + DataKey 的分组策略，你可以根据你的业务来对数据进行分组。 

&ensp;&ensp;合理的分组有助于降低存档的开销。 

&ensp;&ensp;我给出的最理想情况：将经常在同一时间内发生变化的数据划分为一组。

### 7.支持任何复杂类型或自定义类型 

&ensp;&ensp;Flexi Archive System 支持轻松添加新的数据类型和存档策略，允许你存档自定义的复杂类型数据。 

&ensp;&ensp;如果你需要存档一个自定义类型，你无需关心存档过程中所做的操作，也无需对系统进行修改。你只需负责创建一个 CustomData 以及一个 AbstractDataType<CustomData> 具体类型，按照Litjson0.19所支持的类型要求对复杂的数据进行转换。 [具体见LitJson文档](https://litjson.net/blog/2023/11/litjson-v0.19.0-released)

&ensp;&ensp;建议你在编写 Wrapper 的同时对 ToString 方法进行重写，方便数据以明文的形式显示在开发环境中。

代码示例（Plan A）： 
```C#
		/// <summary>
		/// 1.create construct method
		/// </summary>
		public class CustomDataType : AbstractDataType<CustomData>
		{
			public CustomDataType(string dataStr) : base(dataStr){}
		}

		/// <summary>
		/// 2.override "ToString(CustomData)" function if would know detail data
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
```
代码示例（Plan B）：
	
``` C#
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
```

### 8.数据存档监视工具 

&ensp;&ensp;Flexi Archive System 提供了与系统层配套的数据查询工具，方便你在运行时实时的监视数据的变化（支持非运行时和运行时使用）。

![img](https://github.com/wenen-creator/FlexiArchiveSystem/blob/dev/Misc/img/preview02.PNG)

### 9.性能
&ensp;&ensp;Flexi Archive System 系统内部采用了高效的存储机制。默认采用按需载入原则，通过异步IO、缓存机制、分组策略、脏标记等大量优化策略，确保在进行大量频繁的数据操作时，也能够快速响应，尽可能的避免复杂情况下帧率波动等性能问题。

## 关于作者

author: 温文

blog: https://www.unitymake.com

email: yixiangluntan@163.com
