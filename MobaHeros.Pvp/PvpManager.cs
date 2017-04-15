using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public class PvpManager : Singleton<PvpManager>
	{
		private const string LogTag = "pvp";

		public static bool audiotoolediting;

		private bool _isContinuedBattle;

		private bool _isObserver;

		private RoomInfo _roomInfo = new RoomInfo();

		private Task _checkRelive;

		public static bool IsDirectLinkLobby
		{
			get;
			set;
		}

		public BattleInfo BattleInfo
		{
			get;
			private set;
		}

		public BattleRoomInfo ServerBattleRoomInfo
		{
			get;
			set;
		}

		public RoomInfo RoomInfo
		{
			get
			{
				return this._roomInfo;
			}
		}

		public PvpStartGameInfo LoginInfo
		{
			get;
			set;
		}

		public PvpStatisticMgr StatisticMgr
		{
			get;
			private set;
		}

		public PvpJoinType JoinType
		{
			get
			{
				return (this.BattleInfo != null) ? this.BattleInfo.JoinType : PvpJoinType.Invalid;
			}
		}

		public string RoomGid
		{
			get
			{
				if (this.ServerBattleRoomInfo == null)
				{
					return null;
				}
				return this.ServerBattleRoomInfo.roomGid;
			}
		}

		public LoadSceneStatus LoadStatus
		{
			get;
			private set;
		}

		public bool IsGameAbandon
		{
			get;
			private set;
		}

		public bool HasRecvLoadingOk
		{
			get;
			set;
		}

		public bool IsContinuedBattle
		{
			get
			{
				return this._isContinuedBattle;
			}
			set
			{
				this._isContinuedBattle = value;
			}
		}

		public List<string> freeHeros
		{
			get;
			set;
		}

		public int TeamCountMax
		{
			get;
			set;
		}

		public float TotalPlayingSeconds
		{
			get
			{
				if (this.BattleResult != null)
				{
					return (float)TimeSpan.FromTicks(this.BattleResult.gameTime).TotalSeconds;
				}
				DateTime? gameStartTime = this.GameStartTime;
				float result = 0f;
				if (gameStartTime.HasValue)
				{
					result = (float)(DateTime.Now - gameStartTime.Value).TotalSeconds;
				}
				return result;
			}
		}

		public int BattleId
		{
			get
			{
				return (this.BattleInfo != null) ? this.BattleInfo.BattleId : 0;
			}
		}

		public KeyValuePair<int, int> PvpTowerLevel
		{
			get
			{
				this.AssertRoomIsReady();
				int num = this.GetAverage(from x in this._roomInfo.PvpPlayers
				where x.GetTeam() == TeamType.LM
				select x);
				int num2 = this.GetAverage(from x in this._roomInfo.PvpPlayers
				where x.GetTeam() == TeamType.BL
				select x);
				if (num <= 0)
				{
					num = 1;
				}
				if (num2 <= 0)
				{
					num2 = 1;
				}
				return new KeyValuePair<int, int>(num, num2);
			}
		}

		public bool IsSelfConfirmedReady
		{
			get
			{
				ReadyPlayerSampleInfo readyPlayerSampleInfo = this._roomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == this.MyLobbyUserId);
				return readyPlayerSampleInfo != null && readyPlayerSampleInfo.readyChecked;
			}
		}

		private string SceneId
		{
			get
			{
				return (this.BattleId < 80001) ? "80001" : this.BattleId.ToString();
			}
		}

		public bool IsInPvp
		{
			get
			{
				return true;
			}
		}

		public bool IsObserver
		{
			get
			{
				return this._isObserver;
			}
		}

		public int ObserverCount
		{
			get;
			set;
		}

		public DateTime? GameStartTime
		{
			get;
			set;
		}

		public bool IsGlobalObserver
		{
			get
			{
				return this.IsObserver;
			}
		}

		public PvpTeamInfo BattleResult
		{
			get
			{
				this.AssertRoomIsReady();
				return this._roomInfo.BattleResult;
			}
		}

		public TeamType SelfTeamType
		{
			get
			{
				this.AssertRoomIsReady();
				return this._roomInfo.SelfTeam;
			}
		}

		public List<ReadyPlayerSampleInfo> OurPlayers
		{
			get
			{
				return this.GetPlayersByTeam(this.SelfTeamType);
			}
		}

		public int MyLobbyUserId
		{
			get
			{
				this.AssertRoomIsReady();
				return this._roomInfo.MyUserId;
			}
		}

		public int MyHeroUniqueId
		{
			get
			{
				return -this.MyLobbyUserId;
			}
		}

		public IEnumerable<ReadyPlayerSampleInfo> PvpPlayers
		{
			get
			{
				if (!this._roomInfo.IsRoomReady)
				{
					ClientLogger.Error("room is not ready in PvpManager.PvpPlayers");
				}
				return this._roomInfo.PvpPlayers;
			}
		}

		public PvpManager()
		{
			this.ClearState();
		}

		public static void ResetState()
		{
			Singleton<PvpManager>.Instance.ClearState();
		}

		public void SetBattleInfo(int battleId, PvpJoinType joinType)
		{
			this.BattleInfo = new BattleInfo
			{
				BattleId = battleId,
				JoinType = joinType
			};
			string sceneId = this.SceneId;
			LevelManager.m_CurLevel.Set(12, sceneId, sceneId, joinType, 0);
		}

		public void SetBattleInfoWithoutJoinType(int battleId)
		{
			this.SetBattleInfo(battleId, (PvpJoinType)15);
		}

		public void ClearBattleInfo()
		{
			this.BattleInfo = new BattleInfo();
		}

		public void SetRoomInfo(int roomId, int myNewId, ReadyPlayerSampleInfo[] allPlayers, string summonerIdObserverd = null)
		{
			PvpProtocolTools.UpdateHeroSkins(allPlayers);
			this._roomInfo.SetPlayers(roomId, myNewId, allPlayers, summonerIdObserverd);
			this._roomInfo.IsOtherCancelConfirm = false;
			this._isObserver = (this.MyLobbyUserId == -2147483648);
		}

		public void SetRoomInfo(BattleRoomInfo roomInfo, int myNewId, ReadyPlayerSampleInfo[] allPlayers, string summonerIdObserverd = null)
		{
			this.ServerBattleRoomInfo = roomInfo;
			this.SetRoomInfo(roomInfo.roomId, myNewId, allPlayers, summonerIdObserverd);
		}

		public void SetRoomInfoOnServerPve(int inUserId)
		{
			this._roomInfo.SetRoomReady(true);
			this._roomInfo.SetMyUserId(inUserId);
			this._roomInfo.SetSelfTeam(TeamType.LM);
		}

		private void ClearState()
		{
			this.LoadStatus = LoadSceneStatus.NotStart;
			this.BattleInfo = new BattleInfo();
			this._roomInfo = new RoomInfo();
			this.StatisticMgr = new PvpStatisticMgr();
			this.IsGameAbandon = false;
			this.GameStartTime = null;
			this.HasRecvLoadingOk = false;
			this.IsContinuedBattle = false;
			this._isObserver = false;
			Task.Clear(ref this._checkRelive);
		}

		private void AssertRoomIsReady()
		{
			if (!this._roomInfo.IsRoomReady)
			{
				throw new InvalidOperationException("room is not ready");
			}
		}

		public static string GetPvpErrorString(PvpErrorCode error)
		{
			string stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1001");
			switch (error)
			{
			case PvpErrorCode.UnknowError:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1006");
				break;
			case PvpErrorCode.StateError:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1005");
				PvpStateManager.Instance.ChangeState(new PvpStateLoginRecover());
				break;
			case PvpErrorCode.NoRoom:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1002");
				break;
			case PvpErrorCode.WrongPwd:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1008");
				break;
			case PvpErrorCode.UserError:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1007");
				break;
			case PvpErrorCode.NoServer:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1003");
				break;
			case PvpErrorCode.NoUser:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1004");
				break;
			case PvpErrorCode.LobbyColsed:
				stringById = LanguageManager.Instance.GetStringById("Systemprompt_Text_1009");
				break;
			}
			return stringById;
		}

		public string GetSummonerName(int newId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this._roomInfo.PvpPlayers.First((ReadyPlayerSampleInfo obj) => obj.newUid == Mathf.Abs(newId));
			if (readyPlayerSampleInfo == null)
			{
				return null;
			}
			return readyPlayerSampleInfo.userName;
		}

		public int PvpMapNumber()
		{
			switch (this.BattleId)
			{
			case 80001:
				return 1;
			case 80002:
				return 3;
			case 80003:
				return 5;
			case 80004:
				return 2;
			default:
				return 0;
			}
		}

		private int GetAverage(IEnumerable<ReadyPlayerSampleInfo> infos)
		{
			int num = 0;
			int num2 = 0;
			foreach (ReadyPlayerSampleInfo current in infos)
			{
				num += current.level;
				num2++;
			}
			return num / num2;
		}

		public bool SendLobbyMsg(LobbyCode code, params object[] args)
		{
			return this.SendLobbyMsgEx(code, null, args);
		}

		public bool SendLobbyMsgEx(LobbyCode code, SendMsgManager.SendMsgParam param, params object[] args)
		{
			if (PvpManager.IsDirectLinkLobby)
			{
				return this.SendLobbyMsgDirectly(code, args);
			}
			return SendMsgManager.Instance.SendGateLobbyMessage(code, param, args);
		}

		private bool SendLobbyMsgDirectly(LobbyCode code, params object[] args)
		{
			return NetWorkHelper.Instance.client.SendLobbyMsg(code, args);
		}

		public bool IsFromServer(bool isC2P)
		{
			return !isC2P && this.IsInPvp && !isC2P;
		}

		public bool IsPvpSkill(bool isC2P)
		{
			return this.IsInPvp && isC2P;
		}

		public float? GetReliveRatio(int heroUniqueId)
		{
			HeroExtraInRoom heroExtraByUserId = this._roomInfo.GetHeroExtraByUserId(-heroUniqueId);
			if (heroExtraByUserId != null)
			{
				return heroExtraByUserId.ReliveRatio;
			}
			return null;
		}

		public HeroInfoData GetHeroInfoData(int newUseId)
		{
			if (newUseId < 0)
			{
				newUseId = -newUseId;
			}
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(newUseId, "GetHeroInfoData");
			if (readyPlayerSampleInfo == null || readyPlayerSampleInfo.heroInfo == null)
			{
				return null;
			}
			return readyPlayerSampleInfo.heroInfo.Hero;
		}

		public string GetSummonerSkillId(int newUseId)
		{
			if (newUseId < 0)
			{
				newUseId = -newUseId;
			}
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(newUseId, null);
			string unikey = string.Empty;
			if (LevelManager.Instance.IsPvpBattleType)
			{
				if (readyPlayerSampleInfo == null)
				{
					return string.Empty;
				}
				if (string.IsNullOrEmpty(readyPlayerSampleInfo.selfDefSkillId))
				{
					return string.Empty;
				}
				unikey = readyPlayerSampleInfo.selfDefSkillId;
			}
			else
			{
				unikey = Singleton<PvpChoiceSkillView>.Instance.GetSkillID();
			}
			SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(unikey);
			if (dataById == null)
			{
				if (readyPlayerSampleInfo != null)
				{
					ClientLogger.Error("GetSummonerSkillID null:" + readyPlayerSampleInfo.selfDefSkillId);
				}
				return string.Empty;
			}
			return dataById.skill_id;
		}

		public List<ReadyPlayerSampleInfo> GetPlayersByTeam(TeamType team)
		{
			List<ReadyPlayerSampleInfo> list = (from item in this._roomInfo.PvpPlayers
			where item.GetTeam() == team
			select item).ToList<ReadyPlayerSampleInfo>();
			list.Sort((ReadyPlayerSampleInfo x, ReadyPlayerSampleInfo y) => (int)(x.group - y.group));
			return list;
		}

		public TeamType GetTeam(int userId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this._roomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == userId);
			if (readyPlayerSampleInfo != null)
			{
				return readyPlayerSampleInfo.GetTeam();
			}
			return TeamType.None;
		}

		public bool IsOurPlayer(int userId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(userId, null);
			return readyPlayerSampleInfo != null && readyPlayerSampleInfo.GetTeam() == this.SelfTeamType;
		}

		public string GetPlayerHero(int userId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this._roomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == userId);
			if (readyPlayerSampleInfo == null)
			{
				return null;
			}
			return (readyPlayerSampleInfo.heroInfo != null) ? readyPlayerSampleInfo.heroInfo.heroId : null;
		}

		public UnitControlType GetControlType(int uniqueId)
		{
			if (uniqueId < 0)
			{
				return (uniqueId == -this.MyLobbyUserId) ? UnitControlType.PvpMyControl : UnitControlType.PvpNetControl;
			}
			return (!this._roomInfo.CtrlUniqueIds.Contains(uniqueId)) ? UnitControlType.PvpNetControl : UnitControlType.PvpAIControl;
		}

		public bool HasPlayerConfirmedReady(int newUId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this._roomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == newUId);
			return readyPlayerSampleInfo.readyChecked;
		}

		public bool IsMyHero(Units hero)
		{
			return (hero.isHero || hero.isPlayer) && hero.unique_id == this.MyHeroUniqueId;
		}

		private static EntityVo GetVo(ReadyPlayerSampleInfo info)
		{
			if (info == null)
			{
				throw new ArgumentException("info is null");
			}
			if (info.heroInfo == null)
			{
				throw new ArgumentException("heroinfo is null:" + StringUtils.DumpObject(info));
			}
			return new EntityVo(EntityType.Hero, info.GetHeroId(), CharacterDataMgr.instance.GetLevel(info.heroInfo.exp), info.heroInfo.star, info.heroInfo.quality, 0f, 0f, 0, 0);
		}

		private List<EntityVo> GetHeroes(TeamType team)
		{
			List<EntityVo> list = new List<EntityVo>();
			foreach (ReadyPlayerSampleInfo current in this._roomInfo.PvpPlayers)
			{
				if (current.GetTeam() == team)
				{
					EntityVo vo = PvpManager.GetVo(current);
					vo.uid = -current.newUid;
					list.Add(vo);
				}
			}
			return list;
		}

		public int GetHeroCount(TeamType team)
		{
			int num = 0;
			foreach (ReadyPlayerSampleInfo current in this._roomInfo.PvpPlayers)
			{
				if (current.GetTeam() == team)
				{
					num++;
				}
			}
			return num;
		}

		public int GetHeroCountInPlayerSide()
		{
			int num = 0;
			foreach (ReadyPlayerSampleInfo current in this._roomInfo.PvpPlayers)
			{
				if (current.GetTeam() == this.SelfTeamType)
				{
					num++;
				}
			}
			return num;
		}

		public void ChooseTeamGame(int battleId, bool isCustomized)
		{
			this.SetBattleInfo(battleId, (!isCustomized) ? PvpJoinType.Team : PvpJoinType.SefDefineGame);
		}

		public void ChooseSingleGame(string battleId, string hintText = "排队中")
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
			if (dataById == null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("battleId[" + battleId + "] don't has config. error", 1f);
				return;
			}
			this.TeamCountMax = dataById.hero1_number_cap;
			this.ClearState();
			PvpLevelStorage.JoinAsSingle();
			this.SetBattleInfo(int.Parse(battleId), PvpJoinType.Single);
			if (GlobalSettings.Instance.PvpSetting.DirectLinkLobby)
			{
				NetWorkHelper.Instance.ConnectToLobbyServer();
			}
			else
			{
				SendMsgManager.Instance.SendGateLobbyMessage(LobbyCode.C2L_JoinQueue, new SendMsgManager.SendMsgParam(true, hintText, true, 15f), new object[]
				{
					this.BattleInfo.BattleId,
					"10.10.10.87-23000-23001"
				});
			}
		}

		public void LeavePvpQueue()
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_OutQueue, new SendMsgManager.SendMsgParam(true, "离开队列", true, 15f), new object[0]);
		}

		public void ConfirmReady(bool confirm)
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_ReadyCheckOK, new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f), new object[]
			{
				confirm
			});
		}

		public void SelectHero(string heroId)
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_RoomSelectHero, new SendMsgManager.SendMsgParam(true, "选择英雄", true, 15f), new object[]
			{
				heroId
			});
		}

		public void SelectSelfDefSkill(string skillId)
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_SelectSelfDefSkill, new SendMsgManager.SendMsgParam(true, "更换召唤师技能", true, 15f), new object[]
			{
				skillId
			});
		}

		public void SelectHeroFinish()
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_RoomSelectHeroOK, new SendMsgManager.SendMsgParam(true, "确认中", true, 15f), new object[0]);
		}

		public void SelectRandomHero()
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_RoomRandomHero, null, new object[0]);
		}

		public void ReqSwitchHero(int inNewUid)
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_RoomReqChangeHero, null, new object[]
			{
				inNewUid
			});
		}

		public void RespSwitchHero(int inNewUid, bool inIsAccept)
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_RoomRespChangeHero, null, new object[]
			{
				inNewUid,
				(!inIsAccept) ? 0 : 1
			});
		}

		public void ReqCancelSwitchHero(int inNewUid)
		{
			this.SendLobbyMsgEx(LobbyCode.C2L_RoomCancelReqChangeHero, null, new object[]
			{
				inNewUid
			});
		}

		public void LoadPvpSceneBegin()
		{
			if (this.LoadStatus != LoadSceneStatus.NotStart)
			{
				if (this.LoadStatus == LoadSceneStatus.Finished)
				{
					this.LoadPvpSceneEnd();
				}
				return;
			}
			this.LoadStatus = LoadSceneStatus.Started;
			LevelManager.m_CurLevel.SetAllHeroes(new Dictionary<TeamType, List<EntityVo>>
			{
				{
					TeamType.LM,
					this.GetHeroes(TeamType.LM)
				},
				{
					TeamType.BL,
					this.GetHeroes(TeamType.BL)
				},
				{
					TeamType.Team_3,
					this.GetHeroes(TeamType.Team_3)
				}
			});
			ModelManager.Instance.Apply_QualityLevel();
			SceneManager.Instance.ChangeScene(SceneType.Map, true);
		}

		public void LoadPvpSceneEnd()
		{
			this.LoadStatus = LoadSceneStatus.Finished;
			if (GameManager.Instance.ReplayController.IsReplayStart)
			{
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
				return;
			}
			if (this.IsGameAbandon)
			{
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
				return;
			}
			if (this.HasRecvLoadingOk || this.IsObserver)
			{
				SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_QueryInFightInfo, null);
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
			}
			else
			{
				SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_LoadingOK, null);
				SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_QueryInFightInfo, null);
			}
		}

		public void CheckPvpScene()
		{
			if (this.LoadStatus == LoadSceneStatus.Finished)
			{
				this.LoadPvpSceneEnd();
			}
		}

		public void AbandonGame(PvpErrorCode err)
		{
			this.IsGameAbandon = true;
		}

		public void OnSelectHero(int userId, HeroInfo info)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(userId, "OnSelectHero");
			if (readyPlayerSampleInfo != null)
			{
				readyPlayerSampleInfo.heroInfo = info;
				MobaMessageManager.DispatchMsg(ClientC2C.PvpSelectHero, new ParamSelectHero(userId, info));
			}
			else
			{
				ClientLogger.Error("OnSelectHero null hero: " + userId);
			}
		}

		public void OnSelectHeroOk(int newUid, bool ok)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(newUid, "OnSelectHeroOk");
			if (readyPlayerSampleInfo != null)
			{
				readyPlayerSampleInfo.horeSelected = ok;
				if (!ok)
				{
					readyPlayerSampleInfo.heroInfo = null;
				}
				MobaMessageManager.DispatchMsg(ClientC2C.PvpSelectHeroOk, new ParamSelectHero(newUid, readyPlayerSampleInfo.heroInfo));
			}
		}

		public void OnSelectHeroSkin(int userId, string skinId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(userId, "OnSelectHeroSkin");
			if (readyPlayerSampleInfo != null)
			{
				readyPlayerSampleInfo.heroSkinId = skinId;
				HeroSkins.SetHeroSkin(this.GetTeam(userId), this.GetPlayerHero(userId), int.Parse(skinId));
				MobaMessageManager.DispatchMsg(ClientC2C.PvpSelectHeroSkin, new ParamSelectHeroSkin(userId, skinId));
			}
		}

		public void OnSelectHeroSkill(int userId, string skillId)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.TryFindPlayerInfo(userId, null);
			if (readyPlayerSampleInfo != null)
			{
				readyPlayerSampleInfo.selfDefSkillId = skillId;
				MobaMessageManager.DispatchMsg(ClientC2C.PvpSelectHeroSkill, new ParamSelectHeroSkill(userId, skillId));
			}
		}

		public ReadyPlayerSampleInfo TryFindPlayerInfo(int playerId, string reason = null)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this._roomInfo.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == playerId);
			if (readyPlayerSampleInfo == null && reason != null)
			{
				ClientLogger.Error(string.Concat(new object[]
				{
					"cannot found player: ",
					playerId,
					" for ",
					reason,
					" stack:\n",
					new StackTraceUtility().ToString()
				}));
			}
			return readyPlayerSampleInfo;
		}

		public static void On_ReliveHero(int heroId)
		{
			HeroExtraInRoom heroExtraByUserId = Singleton<PvpManager>.Instance._roomInfo.GetHeroExtraByUserId(-heroId);
			if (heroExtraByUserId != null)
			{
				heroExtraByUserId.TimeToRelive = null;
			}
		}

		public static void On_KillHero(int deadId, float passedSeconds, float maxReliveSeconds)
		{
			HeroExtraInRoom heroExtraByUserId = Singleton<PvpManager>.Instance._roomInfo.GetHeroExtraByUserId(-deadId);
			if (heroExtraByUserId != null)
			{
				heroExtraByUserId.TimeToRelive = new DateTime?(DateTime.Now + TimeSpan.FromSeconds((double)(maxReliveSeconds - passedSeconds)));
				heroExtraByUserId.ReliveInterval = TimeSpan.FromSeconds((double)maxReliveSeconds);
			}
		}

		public void PrepareSerializer()
		{
		}

		public void StartCheckRelive(float time)
		{
			Task.Clear(ref this._checkRelive);
			this._checkRelive = new Task(this.CheckRelive_Coroutine(time), true);
		}

		[DebuggerHidden]
		private IEnumerator CheckRelive_Coroutine(float time)
		{
			PvpManager.<CheckRelive_Coroutine>c__Iterator4D <CheckRelive_Coroutine>c__Iterator4D = new PvpManager.<CheckRelive_Coroutine>c__Iterator4D();
			<CheckRelive_Coroutine>c__Iterator4D.time = time;
			<CheckRelive_Coroutine>c__Iterator4D.<$>time = time;
			return <CheckRelive_Coroutine>c__Iterator4D;
		}
	}
}
