using Com.Game.Data;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Com.Game.Manager
{
	public class BaseDataMgr
	{
		private Dictionary<string, Type> clzTypeData;

		private Dictionary<string, Dictionary<string, object>> data;

		private static BaseDataMgr _instance;

		private Dictionary<string, object> _sysSkillMainVos;

		private Dictionary<string, object> _sysGameResVos;

		private Dictionary<string, object> _sysAttrNumberVos;

		private Dictionary<string, object> _sysLanguageVos;

		private Dictionary<string, object> _sysHeroMainVos;

		private Dictionary<string, object> _sysMonsterMainVos;

		public static BaseDataMgr instance
		{
			get
			{
				if (BaseDataMgr._instance == null)
				{
					BaseDataMgr._instance = new BaseDataMgr();
				}
				return BaseDataMgr._instance;
			}
		}

		public bool IsBaseDataInit
		{
			get;
			set;
		}

		public BaseDataMgr()
		{
			BaseDataMgr._instance = this;
			this.clzTypeData = BaseDataConst.registerClzType();
		}

		public Type getClzType(string clzType)
		{
			if (!this.clzTypeData.ContainsKey(clzType))
			{
				throw new NullReferenceException("type null:" + clzType);
			}
			return this.clzTypeData[clzType];
		}

		public void init(object data)
		{
			this.data = (Dictionary<string, Dictionary<string, object>>)data;
		}

		public void parse()
		{
			Name.reset();
			Singleton<DamageDataManager>.Instance.ParseTables();
			Singleton<PerformDataManager>.Instance.ParseTables();
			Singleton<HighEffectDataManager>.Instance.ParseTables();
			Singleton<BuffDataManager>.Instance.ParseTables();
			Singleton<SkillUnitDataMgr>.Instance.ParseTables();
			AudioGameDataLoader.instance.Load();
			this.initDataName<SysSkillMainVo>();
			this.initDataName<SysSkillHigheffVo>();
			this.initDataName<SysSkillBuffVo>();
			this.initDataName<SysSkillPerformVo>();
		}

		private void initDataName<T>()
		{
			Dictionary<string, object> dictionary;
			if (this.data.TryGetValue(typeof(T).Name, out dictionary))
			{
				foreach (string current in dictionary.Keys)
				{
					int num = Name.add(current);
					if (num > 32700)
					{
						Debug.LogError("Name.Key must be short , but now is " + num);
					}
				}
			}
		}

		public void InitBaseConfigData(byte[] bytes)
		{
			if (bytes != null)
			{
				object obj = SerializerUtils.binaryDeserialize(bytes);
				BaseDataMgr.instance.init(obj);
				BaseDataMgr.instance.parse();
				BaseDataMgr.instance.IsBaseDataInit = true;
			}
		}

		public void InitBaseConfigData()
		{
			string assetPath = "Assets/_Resources/Data/bindata.xml";
			TextAsset textAsset = Resources.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			if (textAsset != null)
			{
				object obj = SerializerUtils.binaryDeserialize(textAsset.bytes);
				ClientLogger.AssertNotNull(obj, "bindata is invalid");
				this.init(obj);
				this.parse();
				Resources.UnloadAsset(textAsset);
				BaseDataMgr.instance.IsBaseDataInit = true;
			}
		}

		private string getXMLFolder()
		{
			return Application.dataPath + "/_Resources/Data/BinData/";
		}

		public void InitBaseConfigDataXML()
		{
			string xMLFolder = this.getXMLFolder();
			string[] files = FileUtils.GetFiles(xMLFolder, SearchOption.AllDirectories);
			string text = ".xml";
			if (files != null)
			{
				this.data = new Dictionary<string, Dictionary<string, object>>();
				for (int i = 0; i < files.Length; i++)
				{
					if (text == string.Empty || files[i].Contains(text))
					{
						FileInfo fileInfo = new FileInfo(files[i]);
						string text2 = fileInfo.Name.Split(new char[]
						{
							'.'
						})[0];
						Dictionary<string, object> value = SerializerUtils.loadXML2(files[i], this.getClzType(text2));
						this.data.Add(text2, value);
					}
				}
			}
			this.parse();
			this.IsBaseDataInit = true;
		}

		public void saveModified<T>() where T : class
		{
			Dictionary<string, object> dicByType = this.GetDicByType<T>();
			this.saveXML(typeof(T), dicByType);
		}

		public void saveXML(Type t, Dictionary<string, object> tableDatas)
		{
			string path = this.getXMLFolder() + t.Name + ".xml";
			StringBuilder stringBuilder = new StringBuilder();
			FieldInfo[] fields = t.GetFields();
			foreach (string current in tableDatas.Keys)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				object obj = tableDatas[current];
				FieldInfo[] array = fields;
				for (int i = 0; i < array.Length; i++)
				{
					FieldInfo fieldInfo = array[i];
					bool flag = fieldInfo.Name.Equals(current);
					string content = fieldInfo.GetValue(obj).ToString();
					if (flag)
					{
						stringBuilder2.Append(this.getXMLNode("unikey", content));
					}
					stringBuilder2.Append(this.getXMLNode(fieldInfo.Name, content));
				}
				stringBuilder.Append(this.getXMLNode("item", stringBuilder2.ToString()));
			}
			string xMLNode = this.getXMLNode("root", stringBuilder.ToString());
			File.WriteAllLines(path, new string[]
			{
				xMLNode
			});
		}

		private string getXMLNode(string nodeName, string content)
		{
			string format = "<{0}>{1}</{0}>";
			return string.Format(format, nodeName, content);
		}

		public T GetDataById<T>(string unikey) where T : class
		{
			if (unikey == null)
			{
				return (T)((object)null);
			}
			string name = typeof(T).Name;
			Dictionary<string, object> dictionary;
			if (!this.data.TryGetValue(name, out dictionary))
			{
				return (T)((object)null);
			}
			object obj;
			if (dictionary != null && dictionary.TryGetValue(unikey, out obj))
			{
				return obj as T;
			}
			return (T)((object)null);
		}

		public Dictionary<string, object> GetDicByType<T>() where T : class
		{
			Dictionary<string, object> result;
			try
			{
				result = this.data[typeof(T).Name];
			}
			catch (Exception ex)
			{
				ClientLogger.Warn("warn\n" + ex.Message);
				string content = "配置文件和客户端不匹配，请装新版本，别找wcw!typeof(T).Name= " + typeof(T).Name + ",Info:" + ex.Message;
				CtrlManager.ShowMsgBox("错误", content, null, PopViewType.PopOneButton, "Shit", "取消", null);
				return null;
			}
			return result;
		}

		public Dictionary<string, T> GetTypeDicByType<T>() where T : class
		{
			Dictionary<string, object> dictionary = this.data[typeof(T).Name];
			Dictionary<string, T> dictionary2 = new Dictionary<string, T>();
			foreach (KeyValuePair<string, object> current in dictionary)
			{
				dictionary2[current.Key] = (current.Value as T);
			}
			return dictionary2;
		}

		public Dictionary<string, object> getData(string tableName)
		{
			if (this.data.ContainsKey(tableName))
			{
				return this.data[tableName];
			}
			return null;
		}

		public Dictionary<string, object> getData(Type tt)
		{
			string name = tt.Name;
			return this.getData(name);
		}

		public void LoadDataConfig()
		{
			try
			{
				string path = "Data/BinData/bindata";
				TextAsset textAsset = Resources.Load(path, typeof(TextAsset)) as TextAsset;
				if (textAsset != null)
				{
					object obj = SerializerUtils.binaryDeserialize(textAsset.bytes);
					BaseDataMgr.instance.init(obj);
					BaseDataMgr.instance.parse();
					Resources.UnloadAsset(textAsset);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(" LoadDataConfig Error : " + ex.Message);
			}
		}

		public void SaveDataConfig()
		{
			try
			{
				string path = Application.dataPath + "/Resources/Data/BinData/bindata.xml";
				SerializerUtils.binarySerialize(path, this.data);
			}
			catch (Exception ex)
			{
				Debug.LogError(" SaveDataConfig Error : " + ex.Message);
			}
		}

		public SysSkillMainVo GetSkillMainData(string key)
		{
			if (this._sysSkillMainVos == null)
			{
				this._sysSkillMainVos = this.GetDicByType<SysSkillMainVo>();
			}
			object obj;
			if (this._sysSkillMainVos.TryGetValue(key, out obj))
			{
				return obj as SysSkillMainVo;
			}
			return null;
		}

		public SysGameResVo GetGameResData(string key)
		{
			if (this._sysGameResVos == null)
			{
				this._sysGameResVos = this.GetDicByType<SysGameResVo>();
			}
			object obj;
			if (this._sysGameResVos.TryGetValue(key, out obj))
			{
				return obj as SysGameResVo;
			}
			return null;
		}

		public SysAttrNumberVo GetAttrNumberData(string key)
		{
			if (this._sysAttrNumberVos == null)
			{
				this._sysAttrNumberVos = this.GetDicByType<SysAttrNumberVo>();
			}
			object obj;
			if (this._sysAttrNumberVos.TryGetValue(key, out obj))
			{
				return obj as SysAttrNumberVo;
			}
			return null;
		}

		public SysLanguageVo GetLanguageData(string key)
		{
			if (this._sysLanguageVos == null)
			{
				this._sysLanguageVos = this.GetDicByType<SysLanguageVo>();
			}
			object obj;
			if (this._sysLanguageVos.TryGetValue(key, out obj))
			{
				return obj as SysLanguageVo;
			}
			return null;
		}

		public SysHeroMainVo GetHeroMainData(string key)
		{
			if (this._sysHeroMainVos == null)
			{
				this._sysHeroMainVos = this.GetDicByType<SysHeroMainVo>();
			}
			object obj;
			if (this._sysHeroMainVos.TryGetValue(key, out obj))
			{
				return obj as SysHeroMainVo;
			}
			return null;
		}

		public SysMonsterMainVo GetMonsterMainData(string key)
		{
			if (this._sysMonsterMainVos == null)
			{
				this._sysMonsterMainVos = this.GetDicByType<SysMonsterMainVo>();
			}
			object obj;
			if (this._sysMonsterMainVos.TryGetValue(key, out obj))
			{
				return obj as SysMonsterMainVo;
			}
			return null;
		}
	}
}
