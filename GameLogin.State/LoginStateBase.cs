using Com.Game.Utils;
using MobaClient;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameLogin.State
{
	public class LoginStateBase
	{
		private LoginStateCode loginCode;

		private MobaPeerType peerType;

		private List<ClientNet> listNetMsg = new List<ClientNet>();

		protected LoginStateCode LoginCode
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

		public LoginStateBase(LoginStateCode code, MobaPeerType peer)
		{
			this.LoginCode = code;
			this.PeerType = peer;
		}

		public LoginStateBase(LoginStateCode code)
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

		public virtual void OnUpdate()
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
				else
				{
					ClientLogger.Error("Can't find the method infomation whose name is " + current.ToString());
				}
			}
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
