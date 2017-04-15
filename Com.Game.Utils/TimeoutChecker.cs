using System;
using System.Diagnostics;
using System.Threading;

namespace Com.Game.Utils
{
	public class TimeoutChecker
	{
		private long _timeout;

		private Action<Delegate> _proc;

		private Action<Delegate> _procHandle;

		private Action<Delegate> _timeoutHandle;

		private ManualResetEvent _event = new ManualResetEvent(false);

		public TimeoutChecker(Action<Delegate> proc, Action<Delegate> timeoutHandle)
		{
			this._proc = proc;
			this._timeoutHandle = timeoutHandle;
			this._procHandle = delegate
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				if (this._proc != null)
				{
					this._proc(null);
				}
				stopwatch.Stop();
				if (stopwatch.ElapsedMilliseconds < this._timeout && this._event != null)
				{
					this._event.Set();
				}
			};
		}

		public bool Wait(long timeout)
		{
			this._timeout = timeout;
			this._procHandle.BeginInvoke(null, null, null);
			bool flag = this._event.WaitOne((int)timeout, false);
			if (!flag && this._timeoutHandle != null)
			{
				this._timeoutHandle(null);
			}
			this.Dispose();
			return flag;
		}

		private void Dispose()
		{
			if (this._event != null)
			{
				this._event.Close();
			}
			this._event = null;
			this._proc = null;
			this._procHandle = null;
			this._timeoutHandle = null;
		}

		public void example()
		{
			try
			{
				TimeoutChecker timeoutChecker = new TimeoutChecker(delegate
				{
					try
					{
					}
					catch
					{
					}
				}, delegate
				{
				});
				if (timeoutChecker.Wait(1000L))
				{
				}
			}
			catch (Exception var_1_56)
			{
			}
		}
	}
}
