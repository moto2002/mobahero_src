using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TaskState
{
	public delegate void FinishedHandler(bool manual);

	private IEnumerator mEntity;

	private Coroutine mCoroutine;

	private bool mRunning;

	private bool mPaused;

	private bool mManualStopped;

	public event TaskState.FinishedHandler Finished
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Finished = (TaskState.FinishedHandler)Delegate.Combine(this.Finished, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Finished = (TaskState.FinishedHandler)Delegate.Remove(this.Finished, value);
		}
	}

	public bool Running
	{
		get
		{
			return this.mRunning;
		}
	}

	public bool Paused
	{
		get
		{
			return this.mPaused;
		}
	}

	public Coroutine Routine
	{
		get
		{
			return this.mCoroutine;
		}
	}

	public TaskState(IEnumerator c)
	{
		this.mEntity = c;
		this.mRunning = false;
		this.mPaused = false;
		this.mManualStopped = false;
	}

	public void Start()
	{
		this.mRunning = true;
		this.mCoroutine = TaskManager.Instance.DoStartCoroutine(this.CallWrapper());
	}

	public void Pause()
	{
		this.mPaused = true;
	}

	public void Unpause()
	{
		this.mPaused = false;
	}

	public void Stop()
	{
		this.mManualStopped = true;
		this.mRunning = false;
	}

	[DebuggerHidden]
	private IEnumerator CallWrapper()
	{
		TaskState.<CallWrapper>c__Iterator4A <CallWrapper>c__Iterator4A = new TaskState.<CallWrapper>c__Iterator4A();
		<CallWrapper>c__Iterator4A.<>f__this = this;
		return <CallWrapper>c__Iterator4A;
	}
}
