using Com.Game.Module;
using Com.Game.Utils;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace MobaHeros.Pvp.State
{
	public class PvpStateLoad : PvpStateBase
	{
		public PvpStateLoad() : base(PvpStateCode.PvpLoad)
		{
		}

		private void RegMsgs(PvpStateBase.MsgRegFn func)
		{
			func(PvpCode.C2P_LoadProcess, new MobaMessageFunc(this.P2C_LoadProcess));
			func(PvpCode.C2P_QueryInFightInfo, new MobaMessageFunc(this.P2C_QueryInFightInfo));
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			Singleton<PvpManager>.Instance.PrepareSerializer();
			Singleton<PvpManager>.Instance.CheckPvpScene();
		}

		protected override void RegistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.RegistMessage));
		}

		protected override void UnregistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.UnRegistMessage));
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			if (GameManager.Instance.ReplayController.IsReplayStart)
			{
				return;
			}
			base.OnDisconnectServer(type);
			if (type == MobaPeerType.C2PvpServer)
			{
				PvpStateManager.Instance.ChangeState(new PvpStateRecover(PvpStateCode.PvpLoad));
			}
		}

		private void P2C_LoadProcess(MobaMessage msg)
		{
			Shader.WarmupAllShaders();
			C2PLoadProcess probufMsg = msg.GetProbufMsg<C2PLoadProcess>();
			int newUid = probufMsg.newUid;
			byte process = probufMsg.process;
			PvpStateBase.LogState(string.Concat(new object[]
			{
				"===>receive: P2C_LoadProcess:",
				newUid,
				" ",
				process
			}));
			HeroExtraInRoom heroExtraByUserId = Singleton<PvpManager>.Instance.RoomInfo.GetHeroExtraByUserId(newUid);
			if (heroExtraByUserId != null)
			{
				heroExtraByUserId.LoadProgress = (int)process;
			}
			MobaMessageManager.DispatchMsg(ClientC2C.PvpLoadProcess, new ParamLoadProcess(newUid, process));
		}

		protected override void OnAfterLoadOk(InBattleRuntimeInfo info)
		{
			if (Singleton<PvpManager>.Instance.LoadStatus == LoadSceneStatus.Finished)
			{
				this.SyncFightInfo(info);
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
				FrameSyncManager.Instance.ResetInfoOnOnBattleStart();
			}
		}

		private void P2C_QueryInFightInfo(MobaMessage msg)
		{
			InBattleRuntimeInfo probufMsg = msg.GetProbufMsg<InBattleRuntimeInfo>();
			PvpStateBase.LogState("receive P2C_QueryInFightInfo " + StringUtils.DumpObject(probufMsg));
			this.SyncFightInfo(probufMsg);
		}

		private void SyncFightInfo(InBattleRuntimeInfo fightInfo)
		{
			if (Singleton<PvpManager>.Instance.LoadStatus != LoadSceneStatus.Finished)
			{
				return;
			}
			if (fightInfo == null)
			{
				this.TryStartObserving();
				PvpStateBase.LogState("no fight info");
				return;
			}
			RoomState roomState = (RoomState)fightInfo.roomState;
			PvpStateBase.LogState("roomstate " + roomState);
			switch (roomState)
			{
			case RoomState.Wait:
				this.TryStartObserving();
				break;
			case RoomState.Fighting:
			case RoomState.LogicStart:
				PvpProtocolTools.SyncFightInfo(fightInfo);
				this.TryStartObserving();
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
				break;
			case RoomState.End:
				Singleton<PvpManager>.Instance.AbandonGame(PvpErrorCode.StateError);
				break;
			default:
				ClientLogger.Assert(false, "unknown roomstate " + roomState);
				break;
			}
		}

		private void TryStartObserving()
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				PvpStateBase.LogState("observing is starting");
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
			}
		}
	}
}
