using Com.Game.Module;
using MobaMessageData;
using System;
using UnityEngine;

internal class SceneLoader
{
	protected SceneType _sceneType;

	protected SceneManager Smng;

	protected string strSceneName;

	public SceneType SceneTypeVal
	{
		get
		{
			return this._sceneType;
		}
	}

	public SceneLoader(SceneType s, SceneManager m, string strName)
	{
		this.strSceneName = strName;
		this._sceneType = s;
		this.Smng = m;
	}

	public virtual void Unload()
	{
	}

	public virtual void ClearViews()
	{
	}

	public virtual void BeginLoad()
	{
	}

	public virtual void LoadScene()
	{
		if (!string.IsNullOrEmpty(this.strSceneName))
		{
			Application.LoadLevel(this.strSceneName);
		}
	}

	public virtual void SetCamera()
	{
		CameraRoot.Instance.SetCamera(this._sceneType);
	}

	public virtual void SetFPS()
	{
		CameraRoot.SetTargetFPS(30);
	}

	public virtual void SetManager()
	{
	}

	public virtual void OpenViews()
	{
	}

	public virtual void LoadSceneEnd()
	{
	}

	protected virtual void SendTargetProgress(int targetNum)
	{
		MsgData_LoadView_setProgress data = new MsgData_LoadView_setProgress(MsgData_LoadView_setProgress.SetType.targetNum, targetNum);
		this.SendProgress(data);
	}

	protected virtual void SendAddProgress(int addNum)
	{
		MsgData_LoadView_setProgress data = new MsgData_LoadView_setProgress(MsgData_LoadView_setProgress.SetType.addNum, addNum);
		this.SendProgress(data);
	}

	private void SendProgress(MsgData_LoadView_setProgress data)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2V.LoadView_setProgress, data, true);
	}

	protected void SendSceneLoadComplete()
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.SceneLoadComplete, (int)this._sceneType, false);
	}

	protected void SendLoadingText(string str)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2V.LoadView_setText, str, true);
	}

	protected void SetGoOn()
	{
		this.Smng.GoOnFlag = true;
	}
}
