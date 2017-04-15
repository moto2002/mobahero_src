using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class SummonerSkillView
	{
		private Transform transform;

		private UIScrollView L_Panel;

		private UIGrid L_Grid;

		private UISprite R_Mask;

		private UITexture R_Texture;

		private UITexture R_Performance;

		private UILabel R_Name;

		private UILabel R_Grade;

		private UILabel R_Introduction;

		private SummonerSkillItem summonerItem;

		private Dictionary<string, SummonerSkillItem> skillObjDict = new Dictionary<string, SummonerSkillItem>();

		private string choseSkillID;

		public SummonerSkillView(Transform trans)
		{
			this.transform = trans;
		}

		public void Init()
		{
			this.summonerItem = Resources.Load<SummonerSkillItem>("Prefab/UI/Summoner/SummonerSkillItem");
			this.L_Panel = this.transform.transform.Find("LeftAnchor/Panel").GetComponent<UIScrollView>();
			this.L_Grid = this.transform.transform.Find("LeftAnchor/Panel/Grid").GetComponent<UIGrid>();
			this.R_Mask = this.transform.transform.Find("RightAnchor/Mask").GetComponent<UISprite>();
			this.R_Texture = this.transform.transform.Find("RightAnchor/Texture").GetComponent<UITexture>();
			this.R_Performance = this.transform.transform.Find("RightAnchor/Performance").GetComponent<UITexture>();
			this.R_Name = this.transform.transform.Find("RightAnchor/Name").GetComponent<UILabel>();
			this.R_Grade = this.transform.transform.Find("RightAnchor/Grade").GetComponent<UILabel>();
			this.R_Introduction = this.transform.transform.Find("RightAnchor/Introduction").GetComponent<UILabel>();
		}

		public void RegisterUpdateHandler()
		{
		}

		public void CancelUpdateHandler()
		{
		}

		public void HandleAfterOpenView()
		{
		}

		public void HandleBeforeCloseView()
		{
		}

		public void RefreshUI()
		{
			this.ShowSkillPanel();
			this.UpdateSkillItem();
		}

		public void Destroy()
		{
		}

		private List<SkillDataItem> GetSummonerSkillList()
		{
			List<SkillDataItem> list = new List<SkillDataItem>();
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersSkillVo>();
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_filed_X("Exp"));
			for (int i = 0; i < dicByType.Keys.Count; i++)
			{
				SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(dicByType.Keys.ElementAt(i));
				SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById.skill_id);
				SkillDataItem item = new SkillDataItem(dataById.id.ToString(), userLevel, dataById.level_limit, LanguageManager.Instance.GetStringById(skillMainData.skill_name), ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false));
				list.Add(item);
			}
			return list;
		}

		private string GetSkillID(int num = -1)
		{
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersSkillVo>();
			SysSummonersSkillVo dataById;
			if (num > dicByType.Keys.Count || num == -1)
			{
				ClientLogger.Error("需要的ID号，超出配置表范围");
				dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(dicByType.Keys.ElementAt(0));
			}
			else
			{
				dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(dicByType.Keys.ElementAt(num));
			}
			return dataById.id.ToString();
		}

		private void ShowSkillPanel()
		{
			List<SkillDataItem> dataList = this.GetSummonerSkillList();
			GridHelper.FillGrid<SummonerSkillItem>(this.L_Grid, this.summonerItem, dataList.Count, delegate(int idx, SummonerSkillItem comp)
			{
				comp.Init(dataList[idx]);
				comp.name = dataList[idx].SkillID;
				comp.ClickSkillItemCallBack = new Callback<GameObject>(this.ClickSkillItem);
				this.skillObjDict[dataList[idx].SkillID] = comp;
			});
		}

		private void UpdateSkillItem()
		{
			for (int i = 0; i < this.skillObjDict.Values.Count; i++)
			{
				this.skillObjDict.Values.ElementAt(i).SetChoseState(false);
			}
			this.skillObjDict[this.GetSkillID(0)].SetChoseState(true);
			this.ShowSkillData(this.GetSkillID(0));
		}

		private void ShowSkillData(string skillID)
		{
			SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(skillID);
			SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById.skill_id);
			this.R_Texture.mainTexture = ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false);
			if ("[]" != dataById.display_picture && string.Empty != dataById.display_picture)
			{
				this.R_Performance.mainTexture = ResourceManager.Load<Texture>(dataById.display_picture, true, true, null, 0, false);
			}
			this.R_Name.text = LanguageManager.Instance.GetStringById(skillMainData.skill_name);
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_filed_X("Exp"));
			if (userLevel < dataById.level_limit)
			{
				this.R_Grade.text = "[ff0000]" + LanguageManager.Instance.GetStringById("SummonerSkillUI_NeedSummonerLevel").Replace("*", dataById.level_limit.ToString());
			}
			else
			{
				this.R_Grade.text = "[00ffcc]" + LanguageManager.Instance.GetStringById("SummonerSkillUI_NeedSummonerLevel").Replace("*", dataById.level_limit.ToString());
			}
			this.R_Introduction.text = LanguageManager.Instance.GetStringById(skillMainData.skill_description);
		}

		private void ClickSkillItem(GameObject obj = null)
		{
			if (this.skillObjDict == null || this.skillObjDict.Keys.Count == 0)
			{
				this.RefreshUI();
			}
			for (int i = 0; i < this.skillObjDict.Values.Count; i++)
			{
				if (obj.name == this.skillObjDict.Values.ElementAt(i).name)
				{
					this.skillObjDict.Values.ElementAt(i).SetChoseState(true);
				}
				else
				{
					this.skillObjDict.Values.ElementAt(i).SetChoseState(false);
				}
			}
			this.ShowSkillData(obj.name);
		}
	}
}
