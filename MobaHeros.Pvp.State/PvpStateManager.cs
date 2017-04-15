using Assets.Scripts.Server;
using GameLogin.State;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobaHeros.Pvp.State
{
	public class PvpStateManager : IGlobalComServer
	{
		public delegate void StateChanged(PvpStateCode oldState, PvpStateCode newState);

		private PvpStateBase _priorSate;

		private PvpStateBase _curState;

		private PvpStateBase _nextState;

		private bool _checkLoginRecovery = true;

		private static PvpStateManager _instance;

		public event PvpStateManager.StateChanged OnStateChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.OnStateChanged = (PvpStateManager.StateChanged)Delegate.Combine(this.OnStateChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.OnStateChanged = (PvpStateManager.StateChanged)Delegate.Remove(this.OnStateChanged, value);
			}
		}

		public static PvpStateManager Instance
		{
			get
			{
				if (PvpStateManager._instance == null)
				{
					Debug.LogError("PvpStateManager is Null");
				}
				return PvpStateManager._instance;
			}
		}

		public PvpStateCode StateCode
		{
			get
			{
				if (this._curState != null)
				{
					return this._curState.StateCode;
				}
				return PvpStateCode.Home;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				" prior:",
				this._priorSate,
				" curr:",
				this._curState,
				" next:",
				this._nextState
			});
		}

		public void OnAwake()
		{
			PvpStateManager._instance = this;
		}

		public void OnStart()
		{
		}

		public void Enable(bool b)
		{
		}

		public void OnRestart()
		{
			this._checkLoginRecovery = true;
			PvpManager.ResetState();
			if (this._curState == null)
			{
				return;
			}
			if (this._curState.StateCode != PvpStateCode.Home)
			{
				this.ChangeState(new PvpStateHome());
			}
		}

		public void OnApplicationQuit()
		{
		}

		public void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnDestroy()
		{
			PvpStateManager._instance = null;
		}

		public void OnApplicationPause(bool isPause)
		{
		}

		public void ChangeState(PvpStateBase state)
		{
			if (this._curState != null && this._curState.StateCode == PvpStateCode.PvpNewbieBegin && state != null && state.StateCode != PvpStateCode.PvpLoad)
			{
				return;
			}
			PvpStateCode stateCode = this.StateCode;
			this._nextState = null;
			if (this._curState != null)
			{
				this._curState.Exit();
			}
			this._curState = state;
			if (this._curState != null)
			{
				this._curState.Enter();
			}
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(stateCode, this.StateCode);
			}
		}

		public void OnUpdate()
		{
			if (this._curState != null)
			{
				this._curState.OnUpdate();
			}
		}

		public void TryLoginRecover()
		{
			if (this._checkLoginRecovery)
			{
				this.ChangeState(new PvpStateLoginRecover());
				this._checkLoginRecovery = false;
			}
		}
	}
}
