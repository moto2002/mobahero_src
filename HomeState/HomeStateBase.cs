using Com.Game.Module;
using MobaClient;
using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HomeState
{
	public class HomeStateBase
	{
		private HomeStateCode loginCode;

		private MobaPeerType peerType;

		private List<ClientNet> listNetMsg = new List<ClientNet>();

		protected HomeStateCode LoginCode
		{
			get
			{
				return this.loginCode;
			}
			private set
			{
				this.loginCode = value;
			}
		}

		protected MobaPeerType PeerType
		{
			get
			{
				return this.peerType;
			}
			private set
			{
				this.peerType = value;
				this.listNetMsg.Clear();
				if (this.peerType == MobaPeerType.C2GateServer)
				{
					this.listNetMsg.Add(ClientNet.Connected_game);
					this.listNetMsg.Add(ClientNet.Disconnected_game);
				}
				else if (this.peerType == MobaPeerType.C2Master)
				{
					this.listNetMsg.Add(ClientNet.Connected_master);
					this.listNetMsg.Add(ClientNet.Disconnected_master);
				}
			}
		}

		public HomeStateBase(HomeStateCode code, MobaPeerType peer)
		{
			this.LoginCode = code;
			this.PeerType = peer;
		}

		public HomeStateBase(HomeStateCode code)
		{
			this.LoginCode = code;
		}

		public virtual void OnEnter()
		{
			this.RegistHandler();
		}

		public virtual void OnExit()
		{
			this.UnregistHandler();
		}

		public virtual void OnUpdate(long delta = 0L)
		{
		}

		protected virtual void RegistHandler()
		{
			Type type = base.GetType();
			foreach (ClientNet current in this.listNetMsg)
			{
				MethodInfo method = type.GetMethod(current.ToString(), BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					Delegate @delegate = Delegate.CreateDelegate(typeof(MobaMessageFunc), this, method, false);
					if (@delegate != null)
					{
						MobaMessageManager.RegistMessage((ClientMsg)current, (MobaMessageFunc)@delegate);
					}
				}
			}
			MobaMessageManager.RegistMessage(MobaGateCode.KickOut, new MobaMessageFunc(this.OnKickOut));
		}

		private void OnKickOut(MobaMessage msg)
		{
			NetWorkHelper.Instance.Enable(false);
			CtrlManager.ShowMsgBox("重复登录", "英雄，您在别的地方登录了,传送门将关闭", delegate
			{
				if (GlobalSettings.isLoginByAnySDK)
				{
					InitAnySDK.getInstance().logout();
				}
				else if (GlobalSettings.isLoginByHoolaiSDK)
				{
					InitSDK.instance.SDKLogout(true);
				}
				else
				{
					GlobalObject.ReStartGame();
				}
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}

		protected virtual void UnregistHandler()
		{
			Type type = base.GetType();
			foreach (ClientNet current in this.listNetMsg)
			{
				MethodInfo method = type.GetMethod(current.ToString(), BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					Delegate @delegate = Delegate.CreateDelegate(typeof(MobaMessageFunc), this, method, false);
					if (@delegate != null)
					{
						MobaMessageManager.UnRegistMessage((ClientMsg)current, (MobaMessageFunc)@delegate);
					}
				}
			}
			MobaMessageManager.UnRegistMessage(MobaGateCode.KickOut, new MobaMessageFunc(this.OnKickOut));
		}

		protected virtual void OnConnected_master(MobaMessage msg)
		{
		}

		protected virtual void OnDisconnected_master(MobaMessage msg)
		{
		}

		protected virtual void OnConnected_game(MobaMessage msg)
		{
		}

		protected virtual void OnDisconnected_game(MobaMessage msg)
		{
		}

		protected void Connected_master(MobaMessage msg)
		{
			MobaConnectedType mobaConnectedType = (MobaConnectedType)((int)msg.Param);
			this.OnConnected_master(msg);
		}

		protected void Disconnected_master(MobaMessage msg)
		{
			this.OnDisconnected_master(msg);
		}

		protected void Connected_game(MobaMessage msg)
		{
			MobaConnectedType mobaConnectedType = (MobaConnectedType)((int)msg.Param);
			this.OnConnected_game(msg);
		}

		protected void Disconnected_game(MobaMessage msg)
		{
			this.OnDisconnected_game(msg);
		}
	}
}
