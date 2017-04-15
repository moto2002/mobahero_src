using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CameraMoveTask
{
	private Transform _transform;

	private Vector3 _from;

	public Vector3 To;

	public float Duaration;

	private Task _moveTask;

	public bool Running
	{
		get
		{
			return this._moveTask != null && this._moveTask.Running;
		}
	}

	public CameraMoveTask(Transform t)
	{
		this._transform = t;
	}

	public void MoveFromTo(Vector3 from, Vector3 to, float time)
	{
		this._from = from;
		this.To = to;
		this.Duaration = time;
		if (Math.Abs(this.Duaration) < 0.0001f)
		{
			this._transform.localPosition = to;
		}
		else
		{
			if (this._moveTask != null && this._moveTask.Running)
			{
				this._moveTask.Stop();
			}
			this._moveTask = new Task(this.MakeCenter_Coroutine(), true);
		}
	}

	public void ChangeTo(Vector3 to)
	{
		if (this._moveTask == null || !this._moveTask.Running)
		{
			return;
		}
		this.To = to;
	}

	[DebuggerHidden]
	private IEnumerator MakeCenter_Coroutine()
	{
		CameraMoveTask.<MakeCenter_Coroutine>c__Iterator18 <MakeCenter_Coroutine>c__Iterator = new CameraMoveTask.<MakeCenter_Coroutine>c__Iterator18();
		<MakeCenter_Coroutine>c__Iterator.<>f__this = this;
		return <MakeCenter_Coroutine>c__Iterator;
	}
}
