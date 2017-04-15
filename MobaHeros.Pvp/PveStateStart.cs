using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace MobaHeros.Pvp
{
	public class PveStateStart : PvpStateStart
	{
		public PveStateStart() : base(PvpStateCode.PveStart)
		{
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_LoadingOK, null);
		}

		protected override void RegMsgs(PvpStateBase.MsgRegFn func)
		{
			base.RegMsgs(func);
			func(PvpCode.C2P_QueryInFightInfo, new MobaMessageFunc(this.P2C_QueryInFightInfo));
		}

		protected override void OnAfterLoadOk(InBattleRuntimeInfo info)
		{
			this.SyncFightInfo(info);
			Singleton<PvpManager>.Instance.GameStartTime = new DateTime?(DateTime.Now);
			FrameSyncManager.Instance.ResetInfoOnOnBattleStart();
		}

		private void P2C_QueryInFightInfo(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			byte[] array = (byte[])operationResponse.Parameters[0];
			InBattleRuntimeInfo inBattleRuntimeInfo = SerializeHelper.Deserialize<InBattleRuntimeInfo>(array);
			if (array.Length == 0)
			{
				inBattleRuntimeInfo = null;
			}
			PvpStateBase.LogState("receive P2C_QueryInFightInfo " + StringUtils.DumpObject(inBattleRuntimeInfo));
			this.SyncFightInfo(inBattleRuntimeInfo);
		}

		private void SyncFightInfo(InBattleRuntimeInfo fightInfo)
		{
			if (fightInfo == null)
			{
				PvpStateBase.LogState("no fight info");
				this.TryStartOberving();
				return;
			}
			if (fightInfo.roomState == 3)
			{
				Singleton<PvpManager>.Instance.AbandonGame(PvpErrorCode.StateError);
			}
			else
			{
				PvpProtocolTools.SyncFightInfo(fightInfo);
				this.TryStartOberving();
			}
		}

		private void TryStartOberving()
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				PvpStateBase.LogState("observing is starting");
				PvpStateManager.Instance.ChangeState(new PvpStateStart(PvpStateCode.PvpStart));
			}
		}

		protected override void OnAfterBattleEnd(P2CBattleEndInfo info)
		{
			PvpStateManager.Instance.ChangeState(new PveStateFinish());
		}
	}
}
