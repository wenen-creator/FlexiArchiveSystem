# Flexi Archive System  [![license](https://img.shields.io/badge/%20license-LGPL--2.1-brightgreen?link=https%3A%2F%2Fgithub.com%2Fwenen-creator%2FFlexiArchiveSystem%2Fblob%2Fmaster%2FLICENSE)](https://github.com/wenen-creator/FlexiArchiveSystem/blob/master/LICENSE)  [![license](https://img.shields.io/badge/Author-%E6%B8%A9%E6%96%87-blue?color=%2333A1C9)](https://github.com/wenen-creator)

&ensp;&ensp;*Flexi Archive* is a data archiving tool designed specifically for Unity3D.

&ensp;&ensp;Just like the name *Flexi Archive*, *Flexi Archive* is designed with the concept of *Flexible*, aiming to provide a flexible and powerful solution to meet the complex archiving needs through its characteristics of high scalability, easy to use, high performance and comprehensive tools.

&ensp;&ensp;In *Flexi Archive System* you can easily save almost anything with a few lines of code **cross-platform**, while *Flexi Archive System* with its highly scalable architecture design, Allows you to customize data types and archiving policies and data formats easily according to actual needs.

&ensp;&ensp; *Flexi Archive System* uses **efficient** storage mechanisms. By default, it adopts the principle of on-demand loading, and adopts a large number of optimization strategies such as batching, asynchronous IO, caching mechanism, grouping strategy, dirty mark, etc., to ensure that users can respond quickly when they perform a large number of frequent data operations. Can easily deal with the complex requirements of the game.

## Quick Start

&ensp;&ensp;It is recommended that you open the **Sample** case before using *Flexi Archive*.

&ensp;&ensp;In the Sample case, you can quickly learn the core functions of *Flexi Archive System*.

![img](https://github.com/wenen-creator/FlexiArchiveSystem/blob/dev/Misc/img/preview01.PNG)

## Document

&ensp;&ensp;[Flexi-Archive - Manual](https://www.playcreator.cn/flexi-archive/)

&ensp;&ensp;[Unity-Android application read and write permissions configuration process](https://www.playcreator.cn/archives/unity/3987)

## System features

### 1.Supports multiple archiving mechanisms 

&ensp;&ensp;You can [clone](https://www.playcreator.cn/docs/flexi-archive/manual/clone-archive/) a new archive for the current one when the time is right. The archives are isolated. Modifying one archive does not affect other archives.

&ensp;&ensp;Of course, you can also freely [switch](https://www.playcreator.cn/docs/flexi-archive/manual/switch-archive/), [clone](https://www.playcreator.cn/docs/flexi-archive/manual/clone-archive/), [delete](https://www.playcreator.cn/docs/flexi-archive/manual/del-archive/) the archive.

### 2.Supports saving any complex type or custom type

&ensp;&ensp;Flexi Archive System allows you to easily add new data types and archive policies, allowing you to archive custom complex types of data.

&ensp;&ensp;Currently, the system supports the following types:

|   -----    |  -----  |  -----  |
|:----------:|:-------:|:-------:|
|   float    | double  |   int   |
|    long    |  bool   | string  |
|  vector2   | vector3 | vector4 |
|   object   |  list   |  array  |
| dictionary | custom |  ·····  |

&ensp;&ensp;If you need to archive a custom type, you do not need to care about what you do during the archiving process, and you do not need to make changes to the system. You are only responsible for creating a CustomData and an AbstractDataType<CustomData> concrete type to transform complex data according to the type requirements supported by Litjson0.19 .[See LitJson documentation for details](https://litjson.net/blog/2023/11/litjson-v0.19.0-released)

&ensp;&ensp;It is recommended that you rewrite the ToString method while writing the Wrapper, so that the data can be displayed in plain text in the development environment.

example code
（Plan A）：
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
example code （Plan B）：
See: [Create a custom data type - Flexi Archive](https://www.playcreator.cn/docs/flexi-archive/manual/create-custom-data/)


### 3.Support for object fields and data binding

&ensp;&ensp;In addition to holding an Archive of DataObject operations, Flexi Archive also provides an object field and data binding scheme. You can bind archive data to the properties of the object. This means that your access to and assignment to field members is equivalent to reading and writing to the archived data.

&ensp;&ensp;This binding mechanism simplifies the process of data management, eliminating the need for you to manually write process code to handle data reads and writes, greatly simplifying the archiving process.

&ensp;&ensp;See: [Object fields and data binding - Simplify the archiving process](https://www.playcreator.cn/docs/flexi-archive/manual/properties-bind-data/)


### 4.Supports multiple serialization modes

&ensp;&ensp;Support File, PlayerPrefs, SQL-DB asynchronous serialization (archive/read) mode. Depending on your project's module requirements and performance considerations, you are free to decide which serialization format to use for your module archiving system.The default is SQL-DB serialization.

&ensp;&ensp;Multi-archive support:

1. File mode: Support multiple archives
2. PlayerPrefs approach: Multiple archives are not supported for performance reasons
3. SQL-DB approach: Supports multiple archives

&ensp;&ensp;My analysis：
1. File mode (JSON) : suitable for projects with moderate archive requirements, convenient for cloud archive upload.
2. PlayerPrefs mode： It is suitable for single archive with small amount of data and small amount of archive in each group, and access is faster. For example, record a user operation, the user's local Settings, etc.
3. SQL-DB mode：It is suitable for projects with large amount of archive requirements. The reading overhead and compression ratio are lower than that of File mode in most cases.

### 5.Savepoint

&ensp;&ensp;You need to trigger the save operation at the right time. Otherwise, your modifications will only change the data in Memory.

&ensp;&ensp;It is worth mentioning that Flexi Archive System will only archive data that has changed asynchronously.
```C#
  archiveManager.SaveAsync(() => { Debug.Log("async save successfully");});
```

&ensp;&ensp;See：[Save archive](https://www.playcreator.cn/docs/flexi-archive/manual/save-archive/)

### 6.Use Grouping strategy

&ensp;&ensp;Flexi Archive System uses GroupKey + DataKey grouping strategy, you can group data according to your business.

&ensp;&ensp;Proper grouping helps reduce the overhead of archiving.


### 7.Support the coexistence of multiple account archives on the same device

&ensp;&ensp;Set the USER_KEY after the user has successfully logged in. If your application does not have a login business, you do not need to set it.
example：

``` C#
    void Into() { DataArchiveConstData.USER_KEY = "Wenen"; }
```


### 8.Support for creating multiple archive systems 

&ensp;&ensp;You are free to create as many different archive systems as you want, depending on the module of your application. See：[Creating an archive system](https://www.playcreator.cn/docs/flexi-archive/manual/create-archive-system/)

&ensp;&ensp;It is convenient for you to customize the extension archive system according to the specific module, and you can also choose the serialization archive method that is more suitable for the module.

### 9.Data archive monitoring tool

&ensp;&ensp;The Flexi Archive System provides a data query tool with the system layer, which allows you to monitor data for changes in real time (both non-runtime and runtime).

![img](https://github.com/wenen-creator/FlexiArchiveSystem/blob/dev/Misc/img/preview02.PNG)

### 10.Performance
&ensp;&ensp;Flexi Archive System adopts an efficient storage mechanism. By default, it adopts the principle of on-demand loading, and adopts a large number of optimization strategies such as asynchronous IO, caching mechanism, grouping strategy and dirty mark to ensure that when a large number of frequent data operations are carried out, it can also respond quickly, and avoid performance problems such as frame rate fluctuations in complex situations as much as possible.

## About the Author

author: 温文(Chinese, a game developer)

blog: https://www.playcreator.cn

email: yixiangluntan@163.com
