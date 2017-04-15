using Com.Game.Module;
using Com.Game.Utils;
using GameLogin.State;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;

namespace MobaHeros.Pvp.State
{
	public class PvpStateRecover : PvpStateRecoverBase
	{
		private readonly PvpStateCode _prevState;

		private readonly bool _lostConnectionRecovery;

		private readonly int _queryInterval;

		private Task _queryTask;

		public PvpStateRecover(PvpStateCode prevState) : base(PvpStateCode.PvpRecover, PvpStateRecover.GetLink(prevState))
		{
			this._prevState = prevState;
			this._lostConnectionRecovery = true;
		}

		public PvpStateRecover(PvpStateCode prevState, int queryInterval) : base(PvpStateCode.PvpRecover, PvpStateRecover.GetLink(prevState))
		{
			this._prevState = prevState;
			this._lostConnectionRecovery = false;
			this._queryInterval = queryInterval;
		}

		private static PvpStateRecoverBase.ActiveLink GetLink(PvpStateCode state)
		{
			if (state == PvpStateCode.Home || state == PvpStateCode.PvpInQueue || state == PvpStateCode.PvpSelectHero)
			{
				return PvpStateRecoverBase.ActiveLink.GateServer;
			}
			return PvpStateRecoverBase.ActiveLink.PvpServer;
		}

		protected override void OnL2CQueryPvpState(PlayerState playerState)
		{
			if (playerState == PlayerState.Free)
			{
				base.RecoverFinish(new PvpStateHome());
				NewbieManager.Instance.TryHandleOpenHomeBottomView();
				return;
			}
			if (playerState == PlayerState.InQueue)
			{
				base.RecoverFinish(new PvpStateInQueue());
				return;
			}
			base.SendUserBackToGs();
		}

		protected override void OnP2CBackGame(PvpErrorCode code)
		{
			if (code == PvpErrorCode.OK && (this._prevState == PvpStateCode.PvpLoad || this._prevState == PvpStateCode.PvpSelectHero))
			{
				base.RecoverFinish(new PvpStateLoad());
			}
		}

		protected override void P2C_LoginAsViewer(MobaMessage msg)
		{
			RetaMsg probufMsg = msg.GetProbufMsg<RetaMsg>();
			byte retaCode = probufMsg.retaCode;
			PvpStateBase.LogState("===>receive: P2C_LoginAsViewer:" + retaCode);
			if (retaCode == 0)
			{
				if (this._prevState == PvpStateCode.PveLoad)
				{
					Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
					base.RecoverFinish(new PvpStateLoad());
				}
				else
				{
					base.QueryPsPvpState();
					base.RecoverFinish(new PvpStateStart(PvpStateCode.PvpStart));
				}
			}
			else
			{
				ClientLogger.Error("P2C_LoginAsViewer: failed for " + retaCode);
				Singleton<PvpManager>.Instance.AbandonGame(PvpErrorCode.UnknowError);
			}
		}

		protected override void OnConnectServer(MobaPeerType type)
		{
			base.OnConnectServer(type);
			this.SendQuery();
		}

		private void SendQuery()
		{
			if (base.CurrentLink == PvpStateRecoverBase.ActiveLink.GateServer && (this._prevState == PvpStateCode.Home || this._prevState == PvpStateCode.PvpInQueue || this._prevState == PvpStateCode.PvpSelectHero))
			{
				base.QueryGsPvpState();
				return;
			}
			if (this._prevState == PvpStateCode.PvpStart)
			{
				base.SayHelloToPs((!Singleton<PvpManager>.Instance.IsObserver) ? PvpCode.C2P_Reconnect : PvpCode.C2P_LoginAsViewer);
			}
			else
			{
				base.SayHelloToPs((!Singleton<PvpManager>.Instance.IsObserver) ? PvpCode.C2P_BackGame : PvpCode.C2P_LoginAsViewer);
			}
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			if (!this._lostConnectionRecovery)
			{
				this._queryTask = new Task(new Action(this.SendQuery), 0f, (float)this._queryInterval);
			}
		}

		protected override void OnExit()
		{
			base.OnExit();
			if (!this._lostConnectionRecovery)
			{
				Task.Clear(ref this._queryTask);
			}
		}
	}
}
