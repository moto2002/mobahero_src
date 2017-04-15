using Com.Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MobaHeros.Replay
{
	internal class ReplayMessageIO
	{
		private List<ReplayMessage> _savedMsgs = new List<ReplayMessage>(10000);

		private List<LoadReplayMessage> _loadedMsgs = new List<LoadReplayMessage>(10000);

		private string _path;

		private volatile bool _threadZipping;

		private Exception _threadException;

		private Action<Exception> _finishCallback;

		private int _curLoadMsgIdx;

		private LoadReplayMessage _tempLoadMsg;

		public bool IsHaveElement
		{
			get
			{
				return this._loadedMsgs != null && this._curLoadMsgIdx >= 0 && this._curLoadMsgIdx < this._loadedMsgs.Count;
			}
		}

		public string CurPath
		{
			get
			{
				return this._path;
			}
		}

		public List<LoadReplayMessage> CurLoadMsgs
		{
			get
			{
				return this._loadedMsgs;
			}
		}

		public ReplayMessageIO(string path)
		{
			this._path = path;
		}

		public void Push(ReplayMessage msg)
		{
			this._savedMsgs.Add(msg);
		}

		public void ResetLoadMsgIndex()
		{
			this._curLoadMsgIdx = 0;
		}

		public void MoveNextMsg()
		{
			this._curLoadMsgIdx++;
		}

		public LoadReplayMessage? GetCurrentLoadMsg()
		{
			if (this._loadedMsgs == null)
			{
				return null;
			}
			if (this._curLoadMsgIdx >= 0 && this._curLoadMsgIdx < this._loadedMsgs.Count)
			{
				return new LoadReplayMessage?(this._loadedMsgs[this._curLoadMsgIdx]);
			}
			return null;
		}

		public void Save(Action<Exception> callback)
		{
			if (this._threadZipping)
			{
				ClientLogger.Error("zipping is not finished");
				if (callback != null)
				{
					callback(new InvalidOperationException("zipping is not finished"));
				}
				return;
			}
			List<ReplayMessage> savedMsgs = this._savedMsgs;
			this._savedMsgs = new List<ReplayMessage>(10);
			this._threadException = null;
			this._finishCallback = callback;
			this._threadZipping = true;
			new Thread(delegate(object x)
			{
				List<ReplayMessage> data = x as List<ReplayMessage>;
				try
				{
					SerializerUtils.ReplaySerialize(this._path, data);
				}
				catch (Exception threadException)
				{
					this._threadException = threadException;
				}
				this._threadZipping = false;
			}).Start(savedMsgs);
			new Task(this.Monitor(), true);
		}

		[DebuggerHidden]
		private IEnumerator Monitor()
		{
			ReplayMessageIO.<Monitor>c__Iterator198 <Monitor>c__Iterator = new ReplayMessageIO.<Monitor>c__Iterator198();
			<Monitor>c__Iterator.<>f__this = this;
			return <Monitor>c__Iterator;
		}
	}
}
