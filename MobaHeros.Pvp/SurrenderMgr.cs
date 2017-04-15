using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobaHeros.Pvp
{
	public class SurrenderMgr : BaseGameModule
	{
		public const int MAX_LIMIT = 3;

		public static readonly TimeSpan aliveTime = TimeSpan.FromSeconds(30.0);

		public static readonly TimeSpan cooldown = TimeSpan.FromMinutes(3.0);

		public static readonly TimeSpan takeEffectTime = TimeSpan.FromSeconds(2.0);

		private ActiveSurrenderInfo _activeSurrenderInfo;

		public bool HasAllVoted
		{
			get;
			private set;
		}

		public bool IsPassed
		{
			get;
			private set;
		}

		public int OurTeamCount
		{
			get
			{
				return Singleton<PvpManager>.Instance.OurPlayers.Count;
			}
		}

		public int AcceptCount
		{
			get
			{
				return this._activeSurrenderInfo.votes.Count((ActiveSurrenderInfo.VoteInfo x) => x.accept);
			}
		}

		public int Votes
		{
			get
			{
				return this._activeSurrenderInfo.votes.Count;
			}
		}

		public int ValidVoters
		{
			get
			{
				return this._activeSurrenderInfo.validVoters.Count;
			}
		}

		public float GetLeftAliveTime()
		{
			if (this._activeSurrenderInfo == null)
			{
				return -1f;
			}
			return (float)(SurrenderMgr.aliveTime - (UnitsSnapReporter.Instance.ServerTime - this._activeSurrenderInfo.startTime)).TotalSeconds;
		}

		public float GetSurrenderAvailableTime()
		{
			DateTime? gameStartTime = Singleton<PvpManager>.Instance.GameStartTime;
			TimeSpan minSurrenderTime = this.GetMinSurrenderTime();
			if (!gameStartTime.HasValue)
			{
				return (float)minSurrenderTime.TotalSeconds;
			}
			TimeSpan t = DateTime.Now - gameStartTime.Value;
			float num = (float)(minSurrenderTime - t).TotalSeconds;
			if (num > 0f)
			{
				return num;
			}
			return num;
		}

		private TimeSpan GetMinSurrenderTime()
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurBattleId);
			ClientLogger.AssertNotNull(dataById, "vo is null");
			return TimeSpan.FromMinutes((double)dataById.surrender_time);
		}

		public bool?[] GetTeamMemberStates()
		{
			if (this._activeSurrenderInfo == null)
			{
				return null;
			}
			List<ReadyPlayerSampleInfo> ourPlayers = Singleton<PvpManager>.Instance.OurPlayers;
			bool?[] array = new bool?[ourPlayers.Count];
			int num = 0;
			foreach (ActiveSurrenderInfo.VoteInfo current in this._activeSurrenderInfo.votes)
			{
				array[num++] = new bool?(current.accept);
			}
			for (int i = num; i < array.Length; i++)
			{
				array[i] = null;
			}
			return array;
		}

		public bool CanCurPlayerVote()
		{
			if (this._activeSurrenderInfo == null)
			{
				return false;
			}
			int curId = Singleton<PvpManager>.Instance.MyLobbyUserId;
			return this._activeSurrenderInfo.votes.Find((ActiveSurrenderInfo.VoteInfo x) => x.userId == curId) == null;
		}

		public void StartSurrender()
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_StartSurrender, null);
		}

		public void VoteSurrender(bool accept)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_VoteSurrender, SerializeHelper.Serialize<SurrenderVoteInfo>(new SurrenderVoteInfo
			{
				accept = accept
			}));
		}

		public KeyValuePair<int, int> GetStats()
		{
			if (this._activeSurrenderInfo == null)
			{
				return default(KeyValuePair<int, int>);
			}
			int key = this._activeSurrenderInfo.votes.Count((ActiveSurrenderInfo.VoteInfo x) => x.accept);
			int value = this._activeSurrenderInfo.votes.Count((ActiveSurrenderInfo.VoteInfo x) => !x.accept);
			return new KeyValuePair<int, int>(key, value);
		}

		public override void Init()
		{
			this._activeSurrenderInfo = null;
			this.HasAllVoted = false;
			this.IsPassed = false;
			MobaMessageManager.RegistMessage(PvpCode.C2P_VoteSurrender, new MobaMessageFunc(this.P2C_VoteSurrender));
			MobaMessageManager.RegistMessage(PvpCode.C2P_StartSurrender, new MobaMessageFunc(this.P2C_StartSurrender));
			MobaMessageManager.RegistMessage(PvpCode.P2C_SurrenderTakeEffect, new MobaMessageFunc(this.P2C_SurrenderTakeEffect));
			MobaMessageManager.RegistMessage((ClientMsg)25040, new MobaMessageFunc(this.OnBattleStart));
		}

		public override void Uninit()
		{
			MobaMessageManager.UnRegistMessage(PvpCode.C2P_VoteSurrender, new MobaMessageFunc(this.P2C_VoteSurrender));
			MobaMessageManager.UnRegistMessage(PvpCode.C2P_StartSurrender, new MobaMessageFunc(this.P2C_StartSurrender));
			MobaMessageManager.UnRegistMessage(PvpCode.P2C_SurrenderTakeEffect, new MobaMessageFunc(this.P2C_SurrenderTakeEffect));
			MobaMessageManager.UnRegistMessage((ClientMsg)25040, new MobaMessageFunc(this.OnBattleStart));
		}

		public void SyncInfos(ActiveSurrenderInfo[] surrenderInfos)
		{
			this._activeSurrenderInfo = null;
			if (surrenderInfos == null || Singleton<PvpManager>.Instance.IsObserver)
			{
				return;
			}
			for (int i = 0; i < surrenderInfos.Length; i++)
			{
				ActiveSurrenderInfo info = surrenderInfos[i];
				this.TryUpdateInfo(info);
			}
		}

		private TeamType IsLm(ActiveSurrenderInfo info)
		{
			return Singleton<PvpManager>.Instance.GetTeam(info.starterUid);
		}

		private void ShowMsg(string msg)
		{
			UIMessageBox.ShowMessage(msg, 1.5f, 0);
		}

		private void ShowError(SurrenderErrorCode err)
		{
			switch (err)
			{
			case SurrenderErrorCode.StartLimit:
				this.ShowMsg(LanguageManager.Instance.GetStringById("BattleSurrender_CanNotSurrender"));
				break;
			case SurrenderErrorCode.StartShortGameTime:
				this.ShowMsg(LanguageManager.Instance.FormatString("BattleSurrender_Tips_ThrowInTheTowel", new object[]
				{
					(int)this.GetMinSurrenderTime().TotalMinutes
				}));
				break;
			case SurrenderErrorCode.StartCD:
				this.ShowMsg(LanguageManager.Instance.GetStringById("BattleSurrender_Tips_UnableToVoteForAShortTime"));
				break;
			case SurrenderErrorCode.StartInProgress:
				this.ShowMsg(LanguageManager.Instance.GetStringById("BattleSurrender_Tips_VoteIsInTheProcess"));
				break;
			case SurrenderErrorCode.VoteAlready:
				this.ShowMsg(LanguageManager.Instance.GetStringById("BattleSurrender_Tips_YouAlreadyVoted"));
				break;
			case SurrenderErrorCode.VoteFinish:
				this.ShowMsg(LanguageManager.Instance.GetStringById("BattleSurrender_Tips_TheVoteIsOver"));
				break;
			}
		}

		private void P2C_SurrenderTakeEffect(MobaMessage msg)
		{
			SurrenderStartInfo probufMsg = msg.GetProbufMsg<SurrenderStartInfo>();
			TeamType teamType = this.IsLm(probufMsg.info);
			if (teamType == Singleton<PvpManager>.Instance.SelfTeamType)
			{
				this.IsPassed = true;
				this.HasAllVoted = true;
			}
			PromptHelper.PromptFormat("167", new object[]
			{
				PromptHelper.GetFriendlyText(teamType)
			});
			CtrlManager.CloseWindow(WindowID.SurrenderView);
		}

		private void P2C_StartSurrender(MobaMessage msg)
		{
			SurrenderStartInfo probufMsg = msg.GetProbufMsg<SurrenderStartInfo>();
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				return;
			}
			if (probufMsg.code != 0)
			{
				this.ShowError((SurrenderErrorCode)probufMsg.code);
				return;
			}
			this.TryUpdateInfo(probufMsg.info);
		}

		private void P2C_VoteSurrender(MobaMessage msg)
		{
			SurrenderStartInfo probufMsg = msg.GetProbufMsg<SurrenderStartInfo>();
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				return;
			}
			if (probufMsg.code != 0)
			{
				this.ShowError((SurrenderErrorCode)probufMsg.code);
				return;
			}
			this.TryUpdateInfo(probufMsg.info);
		}

		private void TryUpdateInfo(ActiveSurrenderInfo info)
		{
			TeamType teamType = this.IsLm(info);
			if (Singleton<PvpManager>.Instance.SelfTeamType == teamType && SurrenderMgr.IsVisible(info))
			{
				this._activeSurrenderInfo = info;
				this.HasAllVoted = (info.validVoters.Count == info.votes.Count);
				CtrlManager.OpenWindow(WindowID.SurrenderView, null);
				MobaMessageManager.DispatchMsg((ClientMsg)23064, info, 0f);
			}
		}

		private void OnBattleStart(MobaMessage msg)
		{
			if (SurrenderMgr.IsVisible(this._activeSurrenderInfo))
			{
				CtrlManager.OpenWindow(WindowID.SurrenderView, null);
			}
		}

		private static bool IsVisible(ActiveSurrenderInfo info)
		{
			return info != null && info.validVoters.Contains(Singleton<PvpManager>.Instance.MyLobbyUserId);
		}
	}
}
