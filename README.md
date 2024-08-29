project: Flexi Archive System
author: ����
blog: https://www.unitymake.com

0.����
	���� Flexi Archive System ����һ����Flexi Archive System �� Flexible Ϊ������ּ��ͨ����߶ȿ���չ�ԡ������֡��������Լ����ƵĹ��ߵ��ص㣬�ṩһ�����㸴�Ӵ浵���������ǿ��Ľ��������
	��Flexi Archive System������������ɵؼ��д����ƽ̨���漸���κζ�����ͬʱFlexi Archive System����߶ȿ���չ�ļܹ���ƣ����������ʵ���������ɵ��Զ����������ͺʹ浵���ԡ����ݸ�ʽ��

1.֧��ͬһ�豸�¶��˺Ŵ浵����
	���û���¼�ɹ�������DataArchiveConstData.USER_KEY���ɡ�����ĳ����޵�¼ҵ������Բ������á�

2.֧�ֶ�浵����
	������ں��ʵ�ʱ����Ϊ��ǰ�浵��¡һ���µĴ浵���浵���Ǹ���ģ��޸�ĳ���浵������������浵���Ӱ�졣
	��Ȼ��Ҳ�������ɵ��л���ɾ���浵��

3.֧�ֶ������л���ʽ
	֧��File��PlayerPrefs��SQL-DB���л����浵/��������ʽ������Ը�����Ŀģ�������Լ����ܿ��������ɾ�����ģ��浵ϵͳ��ʹ�õ����л���ʽ��Ĭ��ΪSQL-DB��ʽ��

	��浵֧�֣�
		File��ʽ��֧�ֶ�浵
		PlayerPrefs��ʽ���������ܿ��ǣ��ݲ�֧�ֶ�浵 
		SQL-DB��ʽ��֧�ֶ�浵
	������
		File��ʽ(JSON)�������ڴ浵���������е���Ŀ�����������ƴ浵�ϴ���
		PlayerPrefs��ʽ�������ڵ���������С��ÿ��浵���ٵĴ浵�����ʽϿ졣���¼���û�ĳ���������û��������������õȡ�
		SQL-DB��ʽ�������ڴ浵������ϴ����Ŀ����ȡ������ѹ�������File��ʽ�ڴ�������Ҫ�͡�

4.֧�ִ�������浵ϵͳ
	����Ը��ݳ���ģ��Ĳ�ͬ�����ɵĴ��������ͬ�Ĵ浵ϵͳ��
	��������ݾ����ģ����������չ�浵ϵͳ��Ҳ����ѡ����ʺϸ�ģ������л��浵��ʽ��

5.�����浵
	����Ҫ�ں��ʵ�ʱ���������浵����������������ݵ��޸ģ�ֻ��ʹMemory�����ݷ����仯��
	ֵ��һ����ǣ�Flexi Archive System ֻ��Է����仯�����ݽ��д浵��

6.�������
	Flexi Archive System ʹ��GroupKey+DataKey�ķ�����ԣ�����Ը������ҵ���������ݽ��з��顣
	����ķ��������ڽ��ʹ浵�Ŀ�����
	�Ҹ������������������������ͬһʱ���ڷ����仯�����ݻ���Ϊһ�顣

7.֧���κθ������ͻ��Զ�������
	Flexi Archive System֧����������µ��������ͺʹ浵���ԣ�������浵�Զ���ĸ����������ݡ�
	�������Ҫ�浵һ���Զ������ͣ���������Ĵ浵�����������Ĳ�����Ҳ�����ϵͳ�����޸ġ���ֻ�踺�𴴽�һ��CustomData�Լ�һ��AbstractDataType<CustomData>�������ͣ�����Litjson��֧�ֵ�����Ҫ��Ը��ӵ����ݽ���ת����
	�������ڱ�дWrapper��ͬʱ��ToString����������д���������������ĵ���ʽ��ʾ�ڿ��������С�
	����ʾ����
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
			public string author = "����";
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
			public string author = "����";
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
			public string author = "����";
			public int code = 1;
			public float luckly = 1.0f;

			public override string ToString()
			{
				return "author: " + author +
					   " code: " + code +
					   " luckly: " + luckly;
			}
		}

8.���ݴ浵���ӹ���
	Flexi Archive System�ṩ����ϵͳ�����׵����ݲ�ѯ���ߣ�������������ʱʵʱ�ļ������ݵı仯��֧�ַ�����ʱ������ʱʹ�ã���

9.����
	��������ʹ��Flexi Archive Systemǰ���ȴ�Sample������
	��Sample�����У�����Կ���ѧϰ��Flexi Archive System�ĺ��Ĺ��ܡ�
	������ᱻFlexi Archive System�����޵���

10.����
	Flexi Archive Systemϵͳ�ڲ������˸�Ч�Ĵ洢���ơ�Ĭ�ϲ��ð�������ԭ�����뻺����ơ�������ԡ����ǵȴ����Ż����ԣ�ȷ���ڽ��д���Ƶ�������ݲ���ʱ��Ҳ�ܹ�������Ӧ�������ܵı��⸴�������֡�ʲ������������⡣

