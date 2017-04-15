using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropAttribute : MonoBehaviour
	{
		[SerializeField]
		private UILabel strAttrType;

		[SerializeField]
		private UILabel strAttrValue;

		private AttrType attrType;

		private int index;

		private int runeType;

		private bool isPercent;

		private string format = string.Empty;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(Dictionary<string, List<int>> dic1, Dictionary<string, List<int>> dic2, int idx)
		{
			this.index = idx;
			List<string> list = new List<string>(dic1.Keys);
			List<string> list2 = new List<string>(dic2.Keys);
			if (this.index >= list.Count)
			{
				this.runeType = 2;
				this.ParseData(list2[this.index - list.Count], dic2[list2[this.index - list.Count]]);
			}
			else
			{
				this.runeType = 1;
				this.ParseData(list[this.index], dic1[list[this.index]]);
			}
		}

		private void ParseData(string attrName, List<int> list)
		{
			float num = 0f;
			string stringById = LanguageManager.Instance.GetStringById("HeroRunsUI_GrowthRuns");
			string stringById2 = LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(attrName).attrName);
			this.strAttrType.text = ((this.runeType != 1) ? (stringById + stringById2) : stringById2);
			for (int num2 = 0; num2 != list.Count; num2++)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(list[num2].ToString());
				string[] array = dataById.attribute.Split(new char[]
				{
					'|'
				});
				string text = array[1].Substring(array[1].Length - 1, 1);
				if (text.CompareTo("%") == 0)
				{
					this.isPercent = true;
					num += float.Parse(array[1].Remove(array[1].Length - 1, 1));
					this.format = string.Empty;
				}
				else
				{
					this.isPercent = false;
					num += float.Parse(array[1]);
					this.format = array[2];
				}
			}
			if (this.isPercent)
			{
				this.strAttrValue.text = "+" + num.ToString() + "%";
			}
			else
			{
				this.strAttrValue.text = "+" + num.ToString(this.format);
			}
		}
	}
}
