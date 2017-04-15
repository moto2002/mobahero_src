using Assets.Scripts.Server;
using HomeState;
using System;

internal class HomeManager : IGlobalComServer
{
	private HomeStateBase _curState;

	private HomeStateBase _nextState;

	private static HomeManager m_instance;

	private bool enable;

	private bool first = true;

	public static HomeManager Instance
	{
		get
		{
			if (HomeManager.m_instance == null)
			{
			}
			return HomeManager.m_instance;
		}
	}

	public void OnAwake()
	{
		HomeManager.m_instance = this;
		this._curState = null;
		this._nextState = null;
	}

	public void OnStart()
	{
		this.ChangeState(HomeStateCode.HomeState_None);
	}

	public void OnDestroy()
	{
		HomeManager.m_instance = null;
	}

	public void Enable(bool b)
	{
		this.enable = b;
		if (this.enable)
		{
			if (this.first)
			{
				this.first = false;
				this.ChangeState(HomeStateCode.HomeState_gameLoginAndRegist);
			}
			else
			{
				this.ChangeState(HomeStateCode.HomeState_menu);
			}
		}
	}

	public void OnRestart()
	{
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnUpdate()
	{
		if (!this.enable)
		{
			return;
		}
		if (this._nextState != null)
		{
			if (this._curState != null)
			{
				this._curState.OnExit();
			}
			this._curState = this._nextState;
			if (this._curState != null)
			{
				this._curState.OnEnter();
				this._nextState = null;
			}
		}
		if (this._curState != null)
		{
			this._curState.OnUpdate(0L);
		}
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	public void ChangeState(HomeStateCode eState)
	{
		Type type = Type.GetType("HomeState." + eState.ToString());
		if (type == null)
		{
			return;
		}
		HomeStateBase homeStateBase = Activator.CreateInstance(type) as HomeStateBase;
		if (homeStateBase == null)
		{
			return;
		}
		this._nextState = homeStateBase;
	}
}
