using Com.Game.Utils;
using MobaClient;
using System;
using System.Threading;

namespace MobaHeros.Server
{
	public abstract class PeerServerBase : ServerHelpCom
	{
		protected PhotonClient _client;

		private bool _connected;

		private string _name;

		protected MobaPeer _peer;

		private Thread _thread;

		private volatile bool _threadDone = true;

		private bool _hasBegun;

		public bool UseMultithread
		{
			get;
			protected set;
		}

		protected bool HasBegun
		{
			get
			{
				return this._hasBegun;
			}
		}

		public override bool ConnectFlag
		{
			get
			{
				return this._connected;
			}
			protected set
			{
				this._connected = value;
			}
		}

		protected PeerServerBase(PhotonClient client, string name)
		{
			this._client = client;
			this._name = name;
			this.UseMultithread = false;
		}

		public override void OnDestroy()
		{
			this.End();
		}

		public override void Begin()
		{
			if (this._hasBegun)
			{
				ClientLogger.Error("already begin " + this._name);
			}
			this._hasBegun = true;
			if (this._peer != null)
			{
				return;
			}
			this._peer = this.ConnectPeer();
			if (this._peer != null)
			{
				this.RegisterCmds();
			}
			this.TryStartThread();
		}

		public override void OnUpdate()
		{
			if (this.UseMultithread)
			{
				return;
			}
			if (this._peer != null)
			{
				this._peer.PeerUpdate();
			}
		}

		public sealed override void End()
		{
			this.TryStopThread();
			if (this._peer != null)
			{
				MobaPeer peer = this._peer;
				this._peer = null;
				peer.PeerDisconnect();
				this.UnregisterCmds();
			}
			this._hasBegun = false;
		}

		private void TryStartThread()
		{
			if (this.UseMultithread && this._thread == null)
			{
				this._threadDone = false;
				this._thread = new Thread(new ParameterizedThreadStart(PeerServerBase.ThreadProc));
				this._thread.Start(this);
			}
		}

		private void TryStopThread()
		{
			if (this._thread != null)
			{
				this._threadDone = true;
				Thread thread = this._thread;
				this._thread = null;
				try
				{
					thread.Join();
				}
				catch (Exception e)
				{
					ClientLogger.LogException(e);
				}
			}
		}

		protected abstract MobaPeer ConnectPeer();

		protected virtual void RegisterCmds()
		{
		}

		protected virtual void UnregisterCmds()
		{
		}

		private static void ThreadProc(object obj)
		{
			PeerServerBase peerServerBase = obj as PeerServerBase;
			while (!peerServerBase._threadDone)
			{
				if (peerServerBase._client != null)
				{
					peerServerBase._client.OnUpdate();
				}
				if (peerServerBase._peer != null)
				{
					peerServerBase._peer.PeerUpdate();
				}
				Thread.Sleep(10);
			}
		}
	}
}
