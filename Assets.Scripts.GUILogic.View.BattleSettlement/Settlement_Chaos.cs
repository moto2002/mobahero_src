using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_Chaos : MonoBehaviour
	{
		public Transform mTop;

		public UILabel mTopTitle;

		public UILabel mTopDescript;

		public Transform mTiger;

		public UILabel mTigerPoint;

		public Transform mLeopard;

		public UILabel mLeopardPoint;

		public Transform mEagle;

		public UILabel mEaglePoint;

		public Dictionary<TeamType, List<ChaosPlayerInfo_Settlement>> mInfoList = new Dictionary<TeamType, List<ChaosPlayerInfo_Settlement>>();

		private CoroutineManager cMgr = new CoroutineManager();

		private bool initFlag;

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			MobaMessageManager.RegistMessage((ClientMsg)23036, new MobaMessageFunc(this.onMsg_showChaos));
		}

		private void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23036, new MobaMessageFunc(this.onMsg_showChaos));
			this.cMgr.StopAllCoroutine();
		}

		private void Init()
		{
			this.initFlag = true;
			this.mInfoList.Clear();
			this.mTop = base.transform.FindChild("Top");
			this.mTopTitle = this.mTop.FindChild("title").GetComponent<UILabel>();
			this.mTopDescript = this.mTop.FindChild("tip").GetComponent<UILabel>();
			this.mTiger = base.transform.FindChild("TigerWidget");
			this.mTigerPoint = this.mTiger.FindChild("point/pointLabel").GetComponent<UILabel>();
			this.mInfoList.Add(TeamType.LM, new List<ChaosPlayerInfo_Settlement>());
			ChaosPlayerInfo_Settlement[] componentsInChildren = this.mTiger.GetComponentsInChildren<ChaosPlayerInfo_Settlement>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ChaosPlayerInfo_Settlement item = componentsInChildren[i];
				this.mInfoList[TeamType.LM].Add(item);
			}
			this.mLeopard = base.transform.FindChild("LeopardWidget");
			this.mLeopardPoint = this.mLeopard.FindChild("point/pointLabel").GetComponent<UILabel>();
			this.mInfoList.Add(TeamType.BL, new List<ChaosPlayerInfo_Settlement>());
			ChaosPlayerInfo_Settlement[] componentsInChildren2 = this.mLeopard.GetComponentsInChildren<ChaosPlayerInfo_Settlement>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				ChaosPlayerInfo_Settlement item2 = componentsInChildren2[j];
				this.mInfoList[TeamType.BL].Add(item2);
			}
			this.mEagle = base.transform.FindChild("EagleWidget");
			this.mEaglePoint = this.mEagle.FindChild("point/pointLabel").GetComponent<UILabel>();
			this.mInfoList.Add(TeamType.Team_3, new List<ChaosPlayerInfo_Settlement>());
			ChaosPlayerInfo_Settlement[] componentsInChildren3 = this.mEagle.GetComponentsInChildren<ChaosPlayerInfo_Settlement>();
			for (int k = 0; k < componentsInChildren3.Length; k++)
			{
				ChaosPlayerInfo_Settlement item3 = componentsInChildren3[k];
				this.mInfoList[TeamType.Team_3].Add(item3);
			}
		}

		private void onMsg_showChaos(MobaMessage msg)
		{
			this.Show();
			this.PlayTween();
		}

		private void SetTitle(TeamType? _winnerTeamType)
		{
			if (!_winnerTeamType.HasValue)
			{
				this.mTopTitle.color = new Color32(244, 0, 0, 255);
				this.mTopTitle.text = "比赛结束!";
				this.mTopDescript.text = "获胜队伍每人获得27钻奖励!";
				return;
			}
			string str = null;
			switch (_winnerTeamType.Value)
			{
			case TeamType.LM:
				str = "猛虎队";
				this.mTopTitle.color = new Color32(244, 0, 161, 255);
				break;
			case TeamType.BL:
				str = "雪豹队";
				this.mTopTitle.color = new Color32(41, 253, 189, 255);
				break;
			case TeamType.Team_3:
				str = "猎鹰队";
				this.mTopTitle.color = new Color32(253, 233, 52, 255);
				break;
			}
			this.mTopTitle.text = str + "获得比赛胜利!";
			this.mTopDescript.text = str + "每人获得27钻奖励!";
		}

		private void Show()
		{
			Settlement_Chaos.<Show>c__AnonStorey244 <Show>c__AnonStorey = new Settlement_Chaos.<Show>c__AnonStorey244();
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
			TeamType? winTeam = Singleton<PvpManager>.Instance.RoomInfo.WinTeam;
			this.SetTitle(new TeamType?(winTeam.Value));
			List<HeroData> heroDataList = this.GetHeroDataList(pvpTeamInfo, 0);
			<Show>c__AnonStorey.victoryPvpList = this.GetVictoryDataList(pvpTeamInfo, 0);
			string b = ModelManager.Instance.Get_userData_filed_X("UserId");
			int j;
			for (j = 0; j < this.mInfoList[TeamType.LM].Count; j++)
			{
				ChaosPlayerInfo_Settlement chaosPlayerInfo_Settlement = this.mInfoList[TeamType.LM][j];
				chaosPlayerInfo_Settlement.Init(heroDataList[j], winTeam.Value == TeamType.LM, <Show>c__AnonStorey.victoryPvpList[j], <Show>c__AnonStorey.victoryPvpList[j].UserID == b, ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == <Show>c__AnonStorey.victoryPvpList[j].SummonerId) != null);
				<Show>c__AnonStorey.victoryPvpList[j].NPCID = heroDataList[j].HeroID;
			}
			this.mTigerPoint.text = this.GetTeamKill(<Show>c__AnonStorey.victoryPvpList);
			heroDataList = this.GetHeroDataList(pvpTeamInfo, 1);
			<Show>c__AnonStorey.victoryPvpList = this.GetVictoryDataList(pvpTeamInfo, 1);
			int k;
			for (k = 0; k < this.mInfoList[TeamType.BL].Count; k++)
			{
				ChaosPlayerInfo_Settlement chaosPlayerInfo_Settlement = this.mInfoList[TeamType.BL][k];
				chaosPlayerInfo_Settlement.Init(heroDataList[k], winTeam.Value == TeamType.BL, <Show>c__AnonStorey.victoryPvpList[k], <Show>c__AnonStorey.victoryPvpList[k].UserID == b, ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == <Show>c__AnonStorey.victoryPvpList[k].SummonerId) != null);
				<Show>c__AnonStorey.victoryPvpList[k].NPCID = heroDataList[k].HeroID;
			}
			this.mLeopardPoint.text = this.GetTeamKill(<Show>c__AnonStorey.victoryPvpList);
			heroDataList = this.GetHeroDataList(pvpTeamInfo, 3);
			<Show>c__AnonStorey.victoryPvpList = this.GetVictoryDataList(pvpTeamInfo, 3);
			int i;
			for (i = 0; i < this.mInfoList[TeamType.Team_3].Count; i++)
			{
				ChaosPlayerInfo_Settlement chaosPlayerInfo_Settlement = this.mInfoList[TeamType.Team_3][i];
				chaosPlayerInfo_Settlement.Init(heroDataList[i], winTeam.Value == TeamType.Team_3, <Show>c__AnonStorey.victoryPvpList[i], <Show>c__AnonStorey.victoryPvpList[i].UserID == b, ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == <Show>c__AnonStorey.victoryPvpList[i].SummonerId) != null);
				<Show>c__AnonStorey.victoryPvpList[i].NPCID = heroDataList[i].HeroID;
			}
			this.mEaglePoint.text = this.GetTeamKill(<Show>c__AnonStorey.victoryPvpList);
		}

		private void PlayTween()
		{
		}

		private string GetTeamKill(List<VictPlayerData> victoryDataList)
		{
			int num = 0;
			if (victoryDataList == null)
			{
				return "0";
			}
			for (int i = 0; i < victoryDataList.Count; i++)
			{
				num += victoryDataList[i].KillHero;
			}
			return num.ToString();
		}

		private List<HeroData> GetHeroDataList(PvpTeamInfo pvpTeamInfo, byte teamType)
		{
			if (pvpTeamInfo == null)
			{
				return null;
			}
			List<HeroData> list = new List<HeroData>();
			List<PvpTeamMember> list2 = new List<PvpTeamMember>();
			list2 = pvpTeamInfo.Teams[teamType];
			if (list2 == null)
			{
				Debug.LogError("SelfTeam is Null" + list2);
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

		private List<VictPlayerData> GetVictoryDataList(PvpTeamInfo pvpTeamInfo, byte teamType)
		{
			if (pvpTeamInfo == null)
			{
				return null;
			}
			List<VictPlayerData> list = new List<VictPlayerData>();
			List<PvpTeamMember> list2 = new List<PvpTeamMember>();
			list2 = pvpTeamInfo.Teams[teamType];
			if (list2 == null)
			{
				Debug.LogError("SelfTeam is Null" + list2);
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
					HeroEquipmentList = (pvpTeamInfo.unitCounters == null || !pvpTeamInfo.unitCounters.Keys.Contains(list2[i].Uid)) ? null : pvpTeamInfo.unitCounters[list2[i].Uid].equiplist,
					SummonerSkillID = Singleton<PvpManager>.Instance.GetSummonerSkillId((!(this.GetUnits(list2[i].baseInfo.userName.ToString()) != null)) ? 0 : this.GetUnits(list2[i].baseInfo.userName.ToString()).unique_id),
					SummonerId = list2[i].baseInfo.SummerId.ToString()
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
	}
}
