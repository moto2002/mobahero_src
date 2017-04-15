using Assets.Scripts.Server;
using System;
using System.Collections;
using UnityEngine;

public class TaskManager : IGlobalComServer
{
	private static TaskManager singleton;

	private GameObject gameObj;

	private ComCoroutine comCoroutine;

	public static TaskManager Instance
	{
		get
		{
			if (TaskManager.singleton == null)
			{
				throw new Exception("TaskManager is null");
			}
			return TaskManager.singleton;
		}
	}

	public void OnAwake()
	{
		TaskManager.singleton = this;
		if (null == this.gameObj)
		{
			this.gameObj = new GameObject("TaskManager");
			this.gameObj.transform.parent = UnityEngine.Object.FindObjectOfType<GlobalObject>().transform;
			this.comCoroutine = this.gameObj.AddComponent<ComCoroutine>();
			UnityEngine.Object.DontDestroyOnLoad(this.gameObj);
		}
	}

	public void OnStart()
	{
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
		this.DoDestroy();
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
		this.DoDestroy();
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	public Coroutine DoStartCoroutine(IEnumerator ie)
	{
		return this.comCoroutine.ComStartCoroutine(ie);
	}

	private void DoDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.gameObj);
		this.gameObj = null;
		TaskManager.singleton = null;
	}

	public static TaskState CreateTask(IEnumerator coroutine)
	{
		return new TaskState(coroutine);
	}
}
