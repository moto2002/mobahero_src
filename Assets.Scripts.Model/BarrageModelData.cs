using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class BarrageModelData
	{
		public Queue<string> mQueue;

		private Dictionary<string, SysBulletScreenVo> mDict;

		private Dictionary<int, List<SysBulletScreenVo>> mDataClassify;

		private Dictionary<string, SysBulletScreenFormatVo> mFormatDict;

		public Dictionary<string, SysBulletScreenVo> dataCfg
		{
			get
			{
				if (this.mDict == null)
				{
					this.InitDictData();
				}
				return this.mDict;
			}
		}

		public Dictionary<int, List<SysBulletScreenVo>> dataClassify
		{
			get
			{
				if (this.mDataClassify == null)
				{
					this.ClassifyData();
				}
				return this.mDataClassify;
			}
		}

		public BarrageModelData()
		{
			this.mQueue = new Queue<string>();
			this.mFormatDict = new Dictionary<string, SysBulletScreenFormatVo>();
		}

		private void InitDictData()
		{
			this.mDict = new Dictionary<string, SysBulletScreenVo>();
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysBulletScreenVo>();
			foreach (KeyValuePair<string, object> current in dicByType)
			{
				this.mDict.Add(current.Key, (SysBulletScreenVo)current.Value);
			}
		}

		private void ClassifyData()
		{
			List<SysBulletScreenVo> list = new List<SysBulletScreenVo>();
			List<SysBulletScreenVo> list2 = new List<SysBulletScreenVo>();
			List<SysBulletScreenVo> list3 = new List<SysBulletScreenVo>();
			List<SysBulletScreenVo> list4 = new List<SysBulletScreenVo>();
			foreach (KeyValuePair<string, SysBulletScreenVo> current in this.dataCfg)
			{
				string[] array = current.Value.use_type.Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string a = array2[i];
					if (a == 1.ToString())
					{
						list.Add(current.Value);
					}
					if (a == 2.ToString())
					{
						list2.Add(current.Value);
					}
					if (a == 3.ToString())
					{
						list3.Add(current.Value);
					}
					if (a == 4.ToString())
					{
						list4.Add(current.Value);
					}
				}
			}
			this.mDataClassify = new Dictionary<int, List<SysBulletScreenVo>>();
			this.mDataClassify.Add(1, list);
			this.mDataClassify.Add(2, list2);
			this.mDataClassify.Add(3, list3);
			this.mDataClassify.Add(4, list4);
		}

		public SysBulletScreenFormatVo GetBarrageFormatById(string _id)
		{
			if (!this.mFormatDict.ContainsKey(_id))
			{
				SysBulletScreenFormatVo dataById = BaseDataMgr.instance.GetDataById<SysBulletScreenFormatVo>(_id);
				this.mFormatDict.Add(_id, dataById);
			}
			return this.mFormatDict[_id];
		}
	}
}
