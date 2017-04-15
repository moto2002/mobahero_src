using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaHeros.Gate;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Pvp.State
{
	public class PvpStateFinish : PvpStateBase
	{
		private readonly CoroutineManager _coroutineManager = new CoroutineManager();

		public PvpStateFinish() : base(PvpStateCode.PvpFinish)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			NetWorkHelper.Instance.DisconnectPvpServer();
			if (NetWorkHelper.Instance.IsGateAvailable)
			{
				this.GateServer_OnConnectedEvent();
			}
			else
			{
				GateReconnection.OnConnectedEvent += new Action(this.GateServer_OnConnectedEvent);
				NetWorkHelper.Instance.GateReconnection.Begin();
			}
		}

		protected override void OnExit()
		{
			base.OnExit();
			GateReconnection.OnConnectedEvent -= new Action(this.GateServer_OnConnectedEvent);
		}

		private void GateServer_OnConnectedEvent()
		{
			this.SendMsg();
		}

		protected override void RegistCallbacks()
		{
			MobaMessageManager.RegistMessage(MobaGameCode.GetPvpFightResult, new MobaMessageFunc(this.GetPvpFightResult));
		}

		protected override void UnregistCallbacks()
		{
			MobaMessageManager.UnRegistMessage(MobaGameCode.GetPvpFightResult, new MobaMessageFunc(this.GetPvpFightResult));
		}

		private void GetPvpFightResult(MobaMessage msg)
		{
			MobaMessageManagerTools.EndWaiting_manual("SendGetPvpFightResult");
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			MobaErrorCode mobaErrorCode = (MobaErrorCode)((int)operationResponse.Parameters[1]);
			if (mobaErrorCode == MobaErrorCode.PvpResultEmpty)
			{
				this._coroutineManager.StartCoroutine(this.RequestAgain(1f), true);
				return;
			}
			if (mobaErrorCode == MobaErrorCode.UserNotExist)
			{
				ClientLogger.Error("GetPvpFightResult: UserNotExist");
				return;
			}
			byte[] buffer = (byte[])operationResponse.Parameters[220];
			PvpTeamInfo pvpTeamInfo = SerializeHelper.Deserialize<PvpTeamInfo>(buffer);
			Singleton<PvpManager>.Instance.RoomInfo.BattleResult = pvpTeamInfo;
			Singleton<PvpManager>.Instance.RoomInfo.OldLadderScore = ModelManager.Instance.Get_userData_X().LadderScore;
			ModelManager.Instance.Get_userData_X().LadderScore = pvpTeamInfo.CurrLadderScore;
			if (GameManager.IsVictory.HasValue && GameManager.IsVictory.Value)
			{
				AnalyticsToolManager.FinishLevel(pvpTeamInfo.sceneid);
			}
			else
			{
				AnalyticsToolManager.FailLevel(pvpTeamInfo.sceneid);
			}
			this._coroutineManager.StopAllCoroutine();
		}

		[DebuggerHidden]
		private IEnumerator RequestAgain(float delay)
		{
			PvpStateFinish.<RequestAgain>c__Iterator50 <RequestAgain>c__Iterator = new PvpStateFinish.<RequestAgain>c__Iterator50();
			<RequestAgain>c__Iterator.delay = delay;
			<RequestAgain>c__Iterator.<$>delay = delay;
			<RequestAgain>c__Iterator.<>f__this = this;
			return <RequestAgain>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator CheckResultReady()
		{
			PvpStateFinish.<CheckResultReady>c__Iterator51 <CheckResultReady>c__Iterator = new PvpStateFinish.<CheckResultReady>c__Iterator51();
			<CheckResultReady>c__Iterator.<>f__this = this;
			return <CheckResultReady>c__Iterator;
		}

		private void onRetryChoice(bool isConfirm)
		{
			if (isConfirm)
			{
				this._coroutineManager.StopAllCoroutine();
				this._coroutineManager.StartCoroutine(this.RequestAgain(0f), true);
				this._coroutineManager.StartCoroutine(this.CheckResultReady(), true);
			}
		}

		private void SendMsg()
		{
			if (Singleton<PvpManager>.Instance.IsObserver || Singleton<BattleSettlementView>.Instance.IsReplay)
			{
				return;
			}
			MobaMessageManagerTools.BeginWaiting_manual("SendGetPvpFightResult", "获取PVP战斗结算...", false, 15f, true);
			if (!SendMsgManager.Instance.SendMsg(MobaGameCode.GetPvpFightResult, null, new object[0]) && ModelManager.Instance != null && ModelManager.Instance.Get_userData_X() != null)
			{
				UnityEngine.Debug.LogError("PvpStateFinish MobaGameCode.GetPvpFightResult failed, userId = " + ModelManager.Instance.Get_userData_X().UserId);
			}
		}
	}
}
