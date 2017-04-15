using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using Newbie;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class PvpChoiceSkillView : BaseView<PvpChoiceSkillView>
	{
		public static string gskillstr = string.Empty;

		private UIGrid P_Grid;

		private UIScrollView P_ScrollView;

		private Transform P_Bg;

		private string P_skillID;

		private SummonerSkillItem summonerItem;

		private Dictionary<string, SummonerSkillItem> skillObjDict = new Dictionary<string, SummonerSkillItem>();

		private Callback<string> clickSkillCallBack;

		public static string editorsummskillid = string.Empty;

		public PvpChoiceSkillView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Summoner/PvpChoiceSkillView");
		}

		public override void Init()
		{
			base.Init();
			this.summonerItem = Resources.Load<SummonerSkillItem>("Prefab/UI/Summoner/SummonerSkillItem");
			this.P_Grid = this.transform.Find("Panel/Grid").GetComponent<UIGrid>();
			this.P_ScrollView = this.transform.Find("Panel").GetComponent<UIScrollView>();
			this.P_Bg = this.transform.Find("Bg2");
			UIEventListener.Get(this.P_Bg.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBackBtn);
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
			this.UpdateSkillList();
			this.ShowSkillData(this.P_skillID);
		}

		private void ClickBackBtn(GameObject obj = null)
		{
			CtrlManager.CloseWindow(WindowID.PvpChoiceSkillView);
		}

		private void ClickSkillItem(GameObject obj = null)
		{
			if (!this.skillObjDict[obj.name].IsCanChoice())
			{
				CtrlManager.CloseWindow(WindowID.TipView);
				Singleton<TipView>.Instance.SetText("技能还未解锁", 0f);
				CtrlManager.OpenWindow(WindowID.TipView, null);
				return;
			}
			this.ShowSkillData(obj.name);
			if (this.clickSkillCallBack != null)
			{
				this.clickSkillCallBack(obj.name);
			}
			CtrlManager.CloseWindow(WindowID.PvpChoiceSkillView);
		}

		private void UpdateSkillList()
		{
			List<SkillDataItem> dataList = this.GetSkillListData(LevelManager.CurLevelId);
			GridHelper.FillGrid<SummonerSkillItem>(this.P_Grid, this.summonerItem, dataList.Count, delegate(int idx, SummonerSkillItem comp)
			{
				comp.Init(dataList[idx]);
				comp.name = dataList[idx].SkillID;
				comp.ClickSkillItemCallBack = new Callback<GameObject>(this.ClickSkillItem);
				this.skillObjDict[dataList[idx].SkillID] = comp;
			});
		}

		private void ShowSkillData(string skillID)
		{
			if (skillID == null)
			{
				return;
			}
			for (int i = 0; i < this.skillObjDict.Values.Count; i++)
			{
				this.skillObjDict.Values.ElementAt(i).SetChoseState(false);
			}
			this.skillObjDict[skillID].SetChoseState(true);
		}

		public void SetSkillID(string skillID, Callback<string> callBack)
		{
			this.P_skillID = skillID;
			this.clickSkillCallBack = callBack;
		}

		public string GetSkillID()
		{
			string text = PvpChoiceSkillView.gskillstr;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return this.P_skillID;
		}

		private List<SkillDataItem> GetSkillListData(string battleId)
		{
			List<SkillDataItem> list = new List<SkillDataItem>();
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_filed_X("Exp"));
			string[] array = dataById.summoners_skill.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				SysSummonersSkillVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(array[i]);
				SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById2.skill_id);
				SkillDataItem item = new SkillDataItem(dataById2.id.ToString(), userLevel, dataById2.level_limit, LanguageManager.Instance.GetStringById(skillMainData.skill_name), ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false));
				list.Add(item);
			}
			return list;
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			NewbieManager.Instance.TryHandleSelSummonerSkill();
		}
	}
}
