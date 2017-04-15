using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Task
{
	public delegate void FinishedHandler(bool manual);

	private TaskState mTaskState;

	public event Task.FinishedHandler Finished
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Finished = (Task.FinishedHandler)Delegate.Combine(this.Finished, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Finished = (Task.FinishedHandler)Delegate.Remove(this.Finished, value);
		}
	}

	public bool Running
	{
		get
		{
			return this.mTaskState.Running;
		}
	}

	public bool Paused
	{
		get
		{
			return this.mTaskState.Paused;
		}
	}

	public Coroutine Routine
	{
		get
		{
			return this.mTaskState.Routine;
		}
	}

	public Task(IEnumerator c, bool autoStart = true)
	{
		this.mTaskState = TaskManager.CreateTask(c);
		if (this.mTaskState != null)
		{
			this.mTaskState.Finished += new TaskState.FinishedHandler(this.TaskFinished);
			if (autoStart)
			{
				this.Start();
			}
		}
	}

	public Task(Action action, float delay) : this(Task.RunOnceWrapper(action, delay, true), true)
	{
	}

	public Task(Action action, float delay, float interval) : this(Task.LoopWrapper(action, delay, interval), true)
	{
	}

	[DebuggerHidden]
	private static IEnumerator RunOnceWrapper(Action action, float delay, bool runonce = true)
	{
		Task.<RunOnceWrapper>c__Iterator48 <RunOnceWrapper>c__Iterator = new Task.<RunOnceWrapper>c__Iterator48();
		<RunOnceWrapper>c__Iterator.delay = delay;
		<RunOnceWrapper>c__Iterator.action = action;
		<RunOnceWrapper>c__Iterator.runonce = runonce;
		<RunOnceWrapper>c__Iterator.<$>delay = delay;
		<RunOnceWrapper>c__Iterator.<$>action = action;
		<RunOnceWrapper>c__Iterator.<$>runonce = runonce;
		return <RunOnceWrapper>c__Iterator;
	}

	[DebuggerHidden]
	private static IEnumerator LoopWrapper(Action action, float delay, float interval)
	{
		Task.<LoopWrapper>c__Iterator49 <LoopWrapper>c__Iterator = new Task.<LoopWrapper>c__Iterator49();
		<LoopWrapper>c__Iterator.delay = delay;
		<LoopWrapper>c__Iterator.action = action;
		<LoopWrapper>c__Iterator.interval = interval;
		<LoopWrapper>c__Iterator.<$>delay = delay;
		<LoopWrapper>c__Iterator.<$>action = action;
		<LoopWrapper>c__Iterator.<$>interval = interval;
		return <LoopWrapper>c__Iterator;
	}

	public void Start()
	{
		if (this.mTaskState != null)
		{
			this.mTaskState.Start();
		}
	}

	public void Pause()
	{
		if (this.mTaskState != null)
		{
			this.mTaskState.Pause();
		}
	}

	public void Unpause()
	{
		if (this.mTaskState != null)
		{
			this.mTaskState.Unpause();
		}
	}

	public void Stop()
	{
		if (this.mTaskState != null)
		{
			this.mTaskState.Stop();
		}
	}

	private void TaskFinished(bool manual)
	{
		Task.FinishedHandler finished = this.Finished;
		if (finished != null)
		{
			finished(manual);
		}
	}

	public static void Clear(ref Task task)
	{
		if (task != null)
		{
			task.Stop();
		}
		task = null;
	}
}
