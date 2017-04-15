using Assets.Scripts.Server;
using Com.Game.Utils;
using GameLogin;
using GameLogin.State;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LoginStateManager : IGlobalComServer
{
	private LoginStateBase _curState;

	private LoginStateBase _nextState;

	private static LoginStateManager m_instance;

	private AccountData tempAccData;

	private bool enable;

	private List<LoginTaskBase> lTask;

	public static LoginStateManager Instance
	{
		get
		{
			if (LoginStateManager.m_instance == null)
			{
				Debug.LogError("LoginStateManager is Null");
			}
			return LoginStateManager.m_instance;
		}
	}

	public AccountData TempAccData
	{
		get
		{
			return this.tempAccData;
		}
		set
		{
			this.tempAccData = value;
		}
	}

	public bool BLogin
	{
		get;
		set;
	}

	public float RSize
	{
		get;
		set;
	}

	public float BSize
	{
		get;
		set;
	}

	public void OnAwake()
	{
		LoginStateManager.m_instance = this;
		this._curState = null;
		this._nextState = null;
	}

	public void OnStart()
	{
		if (!GlobalSettings.isLoginByHoolaiSDK)
		{
			this.ChangeState(LoginStateCode.LoginState_Init);
		}
		else if (InitSDK.instance.IsInit())
		{
			LoginStateManager.Instance.ChangeState(LoginStateCode.LoginState_Init);
		}
	}

	public void OnDestroy()
	{
		LoginStateManager.m_instance = null;
	}

	public void Enable(bool b)
	{
		this.enable = b;
		if (b)
		{
			PlayerMng.Instance.Enable(true);
			if (this.lTask == null)
			{
				this.lTask = new List<LoginTaskBase>();
			}
		}
		else if (this.lTask != null)
		{
			this.lTask.Clear();
			this.lTask = null;
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
			this._nextState = null;
			if (this._curState != null)
			{
				this._curState.OnEnter();
			}
			return;
		}
		if (this._curState != null)
		{
			this._curState.OnUpdate();
		}
		if (this.lTask != null && this.lTask.Count > 0)
		{
			for (int i = this.lTask.Count - 1; i >= 0; i--)
			{
				this.lTask[i].Update();
				if (!this.lTask[i].Valid)
				{
					this.lTask[i].Destroy();
					this.lTask[i] = null;
					this.lTask.RemoveAt(i);
				}
			}
		}
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	public void ChangeState(LoginStateCode eState)
	{
		Type type = Type.GetType("GameLogin.State." + eState.ToString());
		if (type == null)
		{
			ClientLogger.Error("ChangeState：找不到登录状态类型 " + eState.ToString());
			return;
		}
		LoginStateBase loginStateBase = Activator.CreateInstance(type) as LoginStateBase;
		if (loginStateBase == null)
		{
			ClientLogger.Error("ChangeState：无法实例化登录状态 " + eState.ToString());
			return;
		}
		this._nextState = loginStateBase;
	}

	public void AddTask(LoginTaskBase task)
	{
		if (this.lTask != null && task != null)
		{
			this.lTask.Add(task);
		}
	}

	public void AddTasks(params LoginTaskBase[] tasks)
	{
		for (int i = 0; i < tasks.Length; i++)
		{
			this.AddTask(tasks[i]);
		}
	}

	public static void LoginLog(string str)
	{
	}
}
