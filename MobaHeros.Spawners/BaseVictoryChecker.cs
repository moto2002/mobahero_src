using System;
using System.Collections;
using System.Diagnostics;

namespace MobaHeros.Spawners
{
	public abstract class BaseVictoryChecker
	{
		private Task _checkVictoryTask;

		public TeamType WinnerTeam
		{
			get;
			protected set;
		}

		protected BaseVictoryChecker()
		{
			this.WinnerTeam = TeamType.None;
		}

		protected virtual void CheckVictory()
		{
		}

		public virtual void StartCheckVictory()
		{
			this.StopCheckVictory();
			this._checkVictoryTask = new Task(this.CheckVictory_Coroutine(), true);
		}

		public void StopCheckVictory()
		{
			if (this._checkVictoryTask != null)
			{
				this._checkVictoryTask.Stop();
			}
			this._checkVictoryTask = null;
		}

		[DebuggerHidden]
		private IEnumerator CheckVictory_Coroutine()
		{
			BaseVictoryChecker.<CheckVictory_Coroutine>c__Iterator1D0 <CheckVictory_Coroutine>c__Iterator1D = new BaseVictoryChecker.<CheckVictory_Coroutine>c__Iterator1D0();
			<CheckVictory_Coroutine>c__Iterator1D.<>f__this = this;
			return <CheckVictory_Coroutine>c__Iterator1D;
		}
	}
}
