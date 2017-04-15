using Assets.Scripts.Server;
using Com.Game.Utils;
using MobaClient;
using System;
using UnityEngine;

namespace MobaHeros.Gate
{
	public abstract class ReconnectionBase : GlobalComServerBase
	{
		private bool _isBegan;

		protected bool IsBegan
		{
			get
			{
				return this._isBegan;
			}
		}

		public override void OnAwake()
		{
			base.OnAwake();
			MobaMessageManager.RegistMessage((ClientMsg)20001, new MobaMessageFunc(this.HandleNetConnected));
			MobaMessageManager.RegistMessage((ClientMsg)20002, new MobaMessageFunc(this.HandleNetDisconnected));
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			MobaMessageManager.UnRegistMessage((ClientMsg)20001, new MobaMessageFunc(this.HandleNetConnected));
			MobaMessageManager.UnRegistMessage((ClientMsg)20002, new MobaMessageFunc(this.HandleNetDisconnected));
		}

		public virtual void Begin()
		{
			if (this._isBegan)
			{
				ClientLogger.Error("already begins");
				return;
			}
			this._isBegan = true;
			this.RegisterCallbacks();
		}

		public virtual void End(bool success)
		{
			if (!this._isBegan)
			{
				ClientLogger.Error("not begin");
				return;
			}
			this._isBegan = false;
			this.UnregisterCallbacks();
		}

		protected virtual void RegisterCallbacks()
		{
		}

		protected virtual void UnregisterCallbacks()
		{
		}

		protected abstract void OnConnect(PeerConnectedMessage msg);

		protected abstract void OnDisconnect(PeerDisconnectedMessage msg);

		private void HandleNetConnected(MobaMessage msg)
		{
			PeerConnectedMessage peerConnectedMessage = msg.Param as PeerConnectedMessage;
			if (peerConnectedMessage == null)
			{
				Debug.LogError("realMsg == null");
				return;
			}
			if (peerConnectedMessage.ConnectedType == MobaConnectedType.ExceptionOnConnect)
			{
				return;
			}
			this.OnConnect(peerConnectedMessage);
		}

		private void HandleNetDisconnected(MobaMessage msg)
		{
			this.OnDisconnect(msg.Param as PeerDisconnectedMessage);
		}
	}
}
