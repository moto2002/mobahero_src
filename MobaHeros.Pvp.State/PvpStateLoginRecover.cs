using Com.Game.Module;
using GameLogin.State;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace MobaHeros.Pvp.State
{
	public class PvpStateLoginRecover : PvpStateRecoverBase
	{
		private bool _resend;

		public PvpStateLoginRecover() : base(PvpStateCode.PvpLoginRecover, PvpStateRecoverBase.ActiveLink.GateServer)
		{
		}

		protected override void OnEnter()
		{
			Singleton<PvpManager>.Instance.IsContinuedBattle = true;
			base.OnEnter();
			base.QueryGsPvpState();
		}

		protected override void OnL2CQueryPvpState(PlayerState pvpState)
		{
			if (pvpState == PlayerState.Free)
			{
				base.RecoverFinish(new PvpStateHome());
				return;
			}
			if (pvpState == PlayerState.CheckReady || pvpState == PlayerState.SelectHore)
			{
				base.SendUserBackToGs();
			}
			else
			{
				this.PromptToRecover();
			}
		}

		private void PromptToRecover()
		{
			Action<bool> callback = delegate(bool ok)
			{
				if (ok)
				{
					base.SendUserBackToGs();
				}
				else
				{
					CtrlManager.ShowMsgBox("确认吗？", "您可能会因此受到队友举报或者系统逃跑惩罚", delegate(bool okAgain)
					{
						if (okAgain)
						{
							Singleton<PvpManager>.Instance.SendLobbyMsg(LobbyCode.C2L_ForceQuitPvp, new object[0]);
							base.RecoverFinish(new PvpStateHome());
						}
						else
						{
							base.SendUserBackToGs();
						}
					}, PopViewType.PopTwoButton, "确认逃跑", "返回战场", null);
				}
			};
			CtrlManager.ShowMsgBox("提示", "您有一场pvp战斗没有结束，是否继续？", callback, PopViewType.PopTwoButton, "返回战场", "逃跑", null);
		}

		protected override void OnConnectServer(MobaPeerType type)
		{
			base.OnConnectServer(type);
			if (this._resend)
			{
				if (type == MobaPeerType.C2PvpServer)
				{
					PvpStartGameInfo loginInfo = Singleton<PvpManager>.Instance.LoginInfo;
					base.SayHelloToPs((!Singleton<PvpManager>.Instance.IsObserver) ? PvpCode.C2P_BackGame : PvpCode.C2P_LoginAsViewer);
				}
				else
				{
					base.QueryGsPvpState();
				}
			}
			else if (type == MobaPeerType.C2PvpServer)
			{
				PvpStartGameInfo loginInfo2 = Singleton<PvpManager>.Instance.LoginInfo;
				base.SayHelloToPs((!Singleton<PvpManager>.Instance.IsObserver) ? PvpCode.C2P_BackGame : PvpCode.C2P_LoginAsViewer);
			}
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			this._resend = ((type == MobaPeerType.C2PvpServer && base.CurrentLink == PvpStateRecoverBase.ActiveLink.PvpServer) || (type == MobaPeerType.C2GateServer && base.CurrentLink == PvpStateRecoverBase.ActiveLink.GateServer));
		}
	}
}
