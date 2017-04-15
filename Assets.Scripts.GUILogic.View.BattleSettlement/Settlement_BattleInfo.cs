using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_BattleInfo : MonoBehaviour
	{
		private bool initFlag;

		private Dictionary<string, VictoryPVPItem> _victoryPvpItemList = new Dictionary<string, VictoryPVPItem>();

		private VictoryPVPItem _victoryPvpItem;

		private GameObject victoryPvpItemObj;

		private VictoryPVPItem _victoryPvpItemRed;

		private GameObject victoryPvpItemRedObj;

		private Transform mLeft;

		private Transform mRight;

		private UIGrid mLeftGrid;

		private UILabel mLeftClanName;

		private UILabel mLeftTeamData;

		private UIGrid mRightGrid;

		private UILabel mRightClanName;

		private UILabel mRightTeamData;

		private string _reportedPlayerKey;

		private CoroutineManager cMgr = new CoroutineManager();

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			MobaMessageManager.RegistMessage((ClientMsg)23033, new MobaMessageFunc(this.onMsg_showInfo));
			MobaMessageManager.RegistMessage((ClientMsg)21029, new MobaMessageFunc(this.onMsg_showReport));
		}

		private void OnDestroy()
		{
			this.cMgr.StopAllCoroutine();
			MobaMessageManager.UnRegistMessage((ClientMsg)23033, new MobaMessageFunc(this.onMsg_showInfo));
			MobaMessageManager.UnRegistMessage((ClientMsg)21029, new MobaMessageFunc(this.onMsg_showReport));
		}

		private void Init()
		{
			this.initFlag = true;
			this.mLeft = base.transform.Find("Left");
			this.mRight = base.transform.Find("Right");
			this.mLeftGrid = this.mLeft.FindChild("Grid").GetComponent<UIGrid>();
			this.mLeftClanName = this.mLeft.FindChild("ClanName").GetComponent<UILabel>();
			this.mLeftTeamData = this.mLeft.FindChild("Data").GetComponent<UILabel>();
			this.mRightGrid = this.mRight.FindChild("Grid").GetComponent<UIGrid>();
			this.mRightClanName = this.mRight.FindChild("ClanName").GetComponent<UILabel>();
			this.mRightTeamData = this.mRight.FindChild("Data").GetComponent<UILabel>();
			this._victoryPvpItem = Resources.Load<VictoryPVPItem>("Prefab/UI/Victory/VictoryPVPItem");
			this.victoryPvpItemObj = (Resources.Load("Prefab/UI/Victory/VictoryPVPItem") as GameObject);
			this._victoryPvpItemRed = Resources.Load<VictoryPVPItem>("Prefab/UI/Victory/VictoryPVPItemRed");
			this.victoryPvpItemRedObj = (Resources.Load("Prefab/UI/Victory/VictoryPVPItemRed") as GameObject);
		}

		private void onMsg_showInfo(MobaMessage msg)
		{
			if ((int)msg.Param == 12 || Singleton<PvpManager>.Instance.IsObserver)
			{
				this.ShowPvpVictoryView();
			}
			this.cMgr.StartCoroutine(this.PlayTweenPosition(), true);
		}

		private void onMsg_showReport(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			if (flag)
			{
				foreach (KeyValuePair<string, VictoryPVPItem> current in this._victoryPvpItemList)
				{
					current.Value.ShowReport();
				}
			}
			else
			{
				foreach (KeyValuePair<string, VictoryPVPItem> current2 in this._victoryPvpItemList)
				{
					current2.Value.HideReport();
				}
			}
		}

		private void GetData()
		{
			CtrlManager.OpenWindow(WindowID.StatisticView, null);
			Singleton<StatisticView>.Instance.SetModel(false);
			Transform transform = Singleton<StatisticView>.Instance.transform;
			transform.Find("UIPanel").GetComponent<UIPanel>().alpha = 0.01f;
			this.mLeftTeamData.text = transform.FindChild("UIPanel/Left/Data").GetComponent<UILabel>().text;
			this.mRightTeamData.text = transform.FindChild("UIPanel/Right/Data").GetComponent<UILabel>().text;
			UIGrid component = transform.FindChild("UIPanel/Left/Grid").GetComponent<UIGrid>();
			UIGrid component2 = transform.FindChild("UIPanel/Right/Grid").GetComponent<UIGrid>();
			for (int i = 0; i < component.transform.childCount; i++)
			{
				Transform transform2 = NGUITools.AddChild(this.mLeftGrid.gameObject, this.victoryPvpItemObj).transform;
				Transform child = component.transform.GetChild(i);
				transform2.FindChild("AddFriend").gameObject.SetActive(false);
				transform2.FindChild("AddGood").gameObject.SetActive(false);
				transform2.GetComponent<VictoryPVPItem>().RegisteListener();
				transform2.FindChild("SummonerName").GetComponent<UILabel>().text = child.FindChild("name").GetComponent<UILabel>().text;
				transform2.FindChild("Data/KillMonster/Label").GetComponent<UILabel>().text = child.FindChild("KillMonster").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform2.FindChild("Data/KillAssist/Label").GetComponent<UILabel>().text = child.FindChild("AssistInfo").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform2.FindChild("Data/Death/Label").GetComponent<UILabel>().text = child.FindChild("DeathInfo").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform2.FindChild("Data/KillHero/Label").GetComponent<UILabel>().text = child.FindChild("killHeroInfo").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform2.FindChild("HeroItem").GetComponent<UITexture>().mainTexture = child.FindChild("HeroItem").GetComponentInChildren<UITexture>().mainTexture;
			}
			for (int j = 0; j < component2.transform.childCount; j++)
			{
				Transform transform3 = NGUITools.AddChild(this.mRightGrid.gameObject, this.victoryPvpItemRedObj).transform;
				Transform child2 = component2.transform.GetChild(j);
				transform3.FindChild("AddFriend").gameObject.SetActive(false);
				transform3.FindChild("AddGood").gameObject.SetActive(false);
				transform3.GetComponent<VictoryPVPItem>().RegisteListener();
				transform3.FindChild("SummonerName").GetComponent<UILabel>().text = child2.FindChild("name").GetComponent<UILabel>().text;
				transform3.FindChild("Data/KillMonster/Label").GetComponent<UILabel>().text = child2.FindChild("KillMonster").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform3.FindChild("Data/KillAssist/Label").GetComponent<UILabel>().text = child2.FindChild("AssistInfo").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform3.FindChild("Data/Death/Label").GetComponent<UILabel>().text = child2.FindChild("DeathInfo").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform3.FindChild("Data/KillHero/Label").GetComponent<UILabel>().text = child2.FindChild("killHeroInfo").GetComponent<UILabel>().text.Replace("/", string.Empty);
				transform3.FindChild("HeroItem").GetComponent<UITexture>().mainTexture = child2.FindChild("HeroItem").GetComponentInChildren<UITexture>().mainTexture;
			}
			transform.Find("UIPanel").GetComponent<UIPanel>().alpha = 1f;
			CtrlManager.CloseWindow(WindowID.StatisticView);
		}

		[DebuggerHidden]
		private IEnumerator PlayTweenPosition()
		{
			Settlement_BattleInfo.<PlayTweenPosition>c__Iterator104 <PlayTweenPosition>c__Iterator = new Settlement_BattleInfo.<PlayTweenPosition>c__Iterator104();
			<PlayTweenPosition>c__Iterator.<>f__this = this;
			return <PlayTweenPosition>c__Iterator;
		}

		private void ShowPvpVictoryView()
		{
			PvpTeamInfo pvpTeamInfo = ModelManager.Instance.Get_Settle_PvpTeamInfo();
			if (pvpTeamInfo == null)
			{
				pvpTeamInfo = Singleton<PvpManager>.Instance.BattleResult;
			}
			if (pvpTeamInfo == null)
			{
				ClientLogger.Error("Can't get Battle INFOs!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				return;
			}
			List<HeroData> heroDataList = this.GetLMOrBLHeroDataList(pvpTeamInfo, true);
			List<VictPlayerData> victoryPvpList = this.GetLMOrBLVictoryDataList(pvpTeamInfo, true);
			this._victoryPvpItemList.Clear();
			string userId = ModelManager.Instance.Get_userData_filed_X("UserId");
			GridHelper.FillGrid<VictoryPVPItem>(this.mLeftGrid, this._victoryPvpItem, (heroDataList != null) ? heroDataList.Count : 0, delegate(int idx, VictoryPVPItem comp)
			{
				if (victoryPvpList[idx].UserID == userId)
				{
					victoryPvpList[idx].IsControlPlayer = true;
				}
				comp.Init(heroDataList[idx], VictPlayerType.LM, victoryPvpList[idx], victoryPvpList[idx].UserID == userId, ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == victoryPvpList[idx].SummonerId) != null);
				if (this._victoryPvpItemList.ContainsKey(heroDataList[idx].HeroID + "LM"))
				{
					this._victoryPvpItemList.Add(heroDataList[idx].HeroID + idx.ToString() + "LM", comp);
				}
				else
				{
					this._victoryPvpItemList.Add(heroDataList[idx].HeroID + "LM", comp);
				}
				victoryPvpList[idx].NPCID = heroDataList[idx].HeroID;
			});
			this.mLeftGrid.Reposition();
			this.mLeftTeamData.text = string.Concat(new object[]
			{
				this.GetLMHeroKillData(victoryPvpList)[0],
				"/",
				this.GetLMHeroKillData(victoryPvpList)[1],
				"/",
				this.GetLMHeroKillData(victoryPvpList)[2]
			});
			heroDataList = this.GetLMOrBLHeroDataList(pvpTeamInfo, false);
			victoryPvpList = this.GetLMOrBLVictoryDataList(pvpTeamInfo, false);
			GridHelper.FillGrid<VictoryPVPItem>(this.mRightGrid, this._victoryPvpItemRed, (heroDataList != null) ? heroDataList.Count : 0, delegate(int idx, VictoryPVPItem comp)
			{
				if (victoryPvpList[idx].UserID == ModelManager.Instance.Get_userData_X().UserId)
				{
					victoryPvpList[idx].IsControlPlayer = true;
				}
				comp.Init(heroDataList[idx], VictPlayerType.BL, victoryPvpList[idx], victoryPvpList[idx].UserID == userId, ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == victoryPvpList[idx].SummonerId) != null);
				if (this._victoryPvpItemList.ContainsKey(heroDataList[idx].HeroID + "BL"))
				{
					this._victoryPvpItemList.Add(heroDataList[idx].HeroID + idx.ToString() + "BL", comp);
				}
				else
				{
					this._victoryPvpItemList.Add(heroDataList[idx].HeroID + "BL", comp);
				}
				victoryPvpList[idx].NPCID = heroDataList[idx].HeroID;
			});
			this.mRightGrid.Reposition();
			this.mRightTeamData.text = string.Concat(new object[]
			{
				this.GetLMHeroKillData(victoryPvpList)[0],
				"/",
				this.GetLMHeroKillData(victoryPvpList)[1],
				"/",
				this.GetLMHeroKillData(victoryPvpList)[2]
			});
		}

		private List<HeroData> GetLMOrBLHeroDataList(PvpTeamInfo pvpTeamInfo, bool isLM)
		{
			if (pvpTeamInfo == null)
			{
				return null;
			}
			List<HeroData> list = new List<HeroData>();
			List<PvpTeamMember> list2 = new List<PvpTeamMember>();
			if (isLM)
			{
				list2 = pvpTeamInfo.Teams[0];
			}
			else
			{
				list2 = pvpTeamInfo.Teams[1];
			}
			if (list2 == null)
			{
				UnityEngine.Debug.LogError("SelfTeam is Null" + list2);
			}
			for (int i = 0; i < list2.Count; i++)
			{
				HeroData item = new HeroData
				{
					HeroID = list2[i].heroInfo.heroId,
					LV = CharacterDataMgr.instance.GetLevel(list2[i].heroInfo.exp),
					Stars = list2[i].heroInfo.star,
					Quality = list2[i].heroInfo.quality
				};
				list.Add(item);
			}
			return list;
		}

		private List<VictPlayerData> GetLMOrBLVictoryDataList(PvpTeamInfo pvpTeamInfo, bool isLM)
		{
			if (pvpTeamInfo == null)
			{
				return null;
			}
			List<VictPlayerData> list = new List<VictPlayerData>();
			List<PvpTeamMember> list2 = new List<PvpTeamMember>();
			if (isLM)
			{
				list2 = pvpTeamInfo.Teams[0];
			}
			else
			{
				list2 = pvpTeamInfo.Teams[1];
			}
			if (list2 == null)
			{
				UnityEngine.Debug.LogError("SelfTeam is Null" + list2);
			}
			for (int i = 0; i < list2.Count; i++)
			{
				VictPlayerData item = new VictPlayerData
				{
					KillHero = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? 0 : pvpTeamInfo.unitCounters[list2[i].Uid].killHoreCount,
					KillAssist = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? 0 : pvpTeamInfo.unitCounters[list2[i].Uid].helpKillHoreCount,
					KillMonster = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? 0 : pvpTeamInfo.unitCounters[list2[i].Uid].killMonsterCount,
					Gold = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? 0 : pvpTeamInfo.unitCounters[list2[i].Uid].allmoney,
					Death = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? 0 : pvpTeamInfo.unitCounters[list2[i].Uid].deadCount,
					SummonerName = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? string.Empty : list2[i].baseInfo.userName,
					UserID = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? string.Empty : list2[i].Uid,
					HeroEquipmentList = BattleEquipTools_op.GetHeroItemsString(this.GetUnits(list2[i].baseInfo.userName.ToString())),
					SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId((!(this.GetUnits(list2[i].baseInfo.userName.ToString()) != null)) ? 0 : this.GetUnits(list2[i].baseInfo.userName.ToString()).unique_id),
					SummonerId = list2[i].baseInfo.SummerId.ToString(),
					LastChamRank = list2[i].baseInfo.CharmRankvalue
				};
				list.Add(item);
			}
			return list;
		}

		private Units GetUnits(string heroId)
		{
			if (GameManager.Instance.DamageStatisticalManager != null)
			{
				foreach (int current in GameManager.Instance.DamageStatisticalManager.UnitsDictionary.Keys)
				{
					if (heroId == GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current].summonerName)
					{
						return GameManager.Instance.DamageStatisticalManager.UnitsDictionary[current];
					}
				}
			}
			return null;
		}

		private List<int> GetLMHeroKillData(List<VictPlayerData> victoryDataList)
		{
			List<int> list = new List<int>();
			list.Add(0);
			list.Add(0);
			list.Add(0);
			if (victoryDataList == null)
			{
				return list;
			}
			for (int i = 0; i < victoryDataList.Count; i++)
			{
				List<int> list2;
				List<int> expr_2B = list2 = list;
				int num;
				int expr_2E = num = 0;
				num = list2[num];
				expr_2B[expr_2E] = num + victoryDataList[i].KillHero;
				List<int> list3;
				List<int> expr_4C = list3 = list;
				int expr_50 = num = 1;
				num = list3[num];
				expr_4C[expr_50] = num + victoryDataList[i].Death;
				List<int> list4;
				List<int> expr_6F = list4 = list;
				int expr_73 = num = 2;
				num = list4[num];
				expr_6F[expr_73] = num + victoryDataList[i].KillAssist;
			}
			return list;
		}
	}
}
